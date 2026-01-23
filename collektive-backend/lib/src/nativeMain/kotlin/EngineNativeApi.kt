package it.unibo.collektive.unity.core

import it.unibo.collektive.unity.core.network.NetworkManagerImpl
import it.unibo.collektive.unity.data.GlobalData
import it.unibo.collektive.unity.schema.CustomGlobalData
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import it.unibo.collektive.unity.examples.entrypoint
import kotlinx.cinterop.ByteVar
import kotlinx.cinterop.CPointer
import kotlinx.cinterop.ExperimentalForeignApi
import kotlinx.cinterop.IntVar
import kotlinx.cinterop.allocArray
import kotlinx.cinterop.free
import kotlinx.cinterop.set
import kotlinx.cinterop.nativeHeap
import kotlinx.cinterop.pointed
import kotlinx.cinterop.readBytes
import kotlinx.cinterop.value
import kotlin.experimental.ExperimentalNativeApi

var engine: Engine? = null

private fun requireEngine() = require(engine != null) { "The engine is null" }

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("initialize")
fun initialize(dataPointer: CPointer<ByteVar>?, dataSize: Int)
{
    require(dataPointer != null) { "Invalid null pointer. Global data pointer should point to valid heap structure" }
    engine = EngineImpl(
        NetworkManagerImpl(),
        GlobalData.ADAPTER.decode(dataPointer.readBytes(dataSize))
    ) { entrypoint(it) }
}

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("step")
fun step(id: Int, rawSensing: CPointer<ByteVar>, dataSize: Int, outSize: CPointer<IntVar>): CPointer<ByteVar> {
    requireEngine()
    require(dataSize >= 0) { "Invalid data size." }
    val sensingData = SensorData.ADAPTER.decode(rawSensing.readBytes(dataSize))
    val nodeState = engine?.step(id, sensingData)!!
    val byteArray = NodeState.ADAPTER.encode(nodeState)
    val pinnedBuffer = nativeHeap.allocArray<ByteVar>(byteArray.size)
    byteArray.forEachIndexed { index, byte ->
        pinnedBuffer[index] = byte
    }
    outSize.pointed.value = byteArray.size
    return pinnedBuffer
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

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("free_result")
fun freeResult(pointer: CPointer<ByteVar>?) {
    if (pointer != null) {
        nativeHeap.free(pointer)
    }
}
