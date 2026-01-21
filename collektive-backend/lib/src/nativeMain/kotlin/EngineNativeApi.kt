package it.unibo.collektive.unity.core

import it.unibo.collektive.unity.core.network.NetworkManagerImpl
import it.unibo.collektive.unity.data.GlobalData
import it.unibo.collektive.unity.schema.CustomGlobalData
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import kotlinx.cinterop.ByteVar
import kotlinx.cinterop.CPointer
import kotlinx.cinterop.CPointerVar
import kotlinx.cinterop.ExperimentalForeignApi
import kotlinx.cinterop.IntVar
import kotlinx.cinterop.allocArray
import kotlinx.cinterop.get
import kotlinx.cinterop.nativeHeap
import kotlinx.cinterop.plus
import kotlinx.cinterop.pointed
import kotlinx.cinterop.readBytes
import kotlinx.cinterop.refTo
import kotlinx.cinterop.value
import platform.posix.memcpy
import kotlin.experimental.ExperimentalNativeApi

var engine: Engine? = null

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("initialize")
fun initialize(dataPointer: CPointer<ByteVar>?, dataSize: Int)
{
    require(dataPointer != null) { "Invalid null pointer. Global data pointer should point to valid heap structure" }
    engine = EngineImpl(
        NetworkManagerImpl(),
        GlobalData.ADAPTER.decode(dataPointer.readBytes(dataSize))
    )
}

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("step")
fun step(
    count: Int,
    pointers: CPointer<CPointerVar<ByteVar>>?,
    sizes: CPointer<IntVar>?
): CPointer<CPointerVar<ByteVar>> {
    requireEngine()
    require(pointers != null || sizes != null || count > 0)
    { "Invalid data passed to step function. Either pointers or sizes are null or count is less than zero" }
    val sensingData = List(count) { i ->
        val dataPtr = pointers?.get(i)
        val dataSize = sizes?.get(i)
        require(dataSize != null) { "A value inside the sizes list was null (index: $i)" }
        require(dataPtr != null) { "A value inside the data pointers list was null (index: $i)" }
        SensorData.ADAPTER.decode(dataPtr.readBytes(dataSize))
    }
    val nodeStates = engine?.step(sensingData)!!
    val results: List<ByteArray> = nodeStates.map { NodeState.ADAPTER.encode(it) }
    return prepareReturnData(results)
}

private fun requireEngine() = require(engine != null) { "The engine is null" }

@OptIn(ExperimentalForeignApi::class)
private fun prepareReturnData(results: List<ByteArray>): CPointer<CPointerVar<ByteVar>> {
    val resultPointers = nativeHeap.allocArray<CPointerVar<ByteVar>>(results.size)
    results.forEachIndexed { i, bytes ->
        val nativeBytes = nativeHeap.allocArray<ByteVar>(bytes.size)
        memcpy(nativeBytes, bytes.refTo(0), bytes.size.toULong())
        resultPointers.plus(i)!!.pointed.value = nativeBytes
    }
    return resultPointers
}

@OptIn(ExperimentalNativeApi::class)
@CName("add_connection")
fun addConnection(node1: Int, node2: Int): Boolean {
    requireEngine()
    return engine?.addConnection(node1, node2)!!
}

@OptIn(ExperimentalNativeApi::class)
@CName("remove_connection")
fun removeConnection(node1: Int, node2: Int): Boolean {
    requireEngine()
    return engine?.removeConnection(node1, node2)!!
}

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("update_global_data")
fun updateGlobalData(dataPointer: CPointer<ByteVar>?, dataSize: Int)
{
    require(dataPointer != null) { "Invalid null pointer. Global data pointer should point to valid heap structure" }
    engine?.updateGlobalData(CustomGlobalData.ADAPTER.decode(dataPointer.readBytes(dataSize)))
}
