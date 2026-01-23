package it.unibo.collektive.unity.core

import it.unibo.collektive.Collektive
import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.unity.core.network.Network
import it.unibo.collektive.unity.core.network.NetworkManager
import it.unibo.collektive.unity.data.GlobalData
import it.unibo.collektive.unity.examples.entrypoint
import it.unibo.collektive.unity.schema.CustomGlobalData
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData

interface Engine {
    val globalData: GlobalData
    fun step(id: Int, sensorData: SensorData): NodeState
    fun addConnection(node1: Int, node2: Int): Boolean
    fun removeConnection(node1: Int, node2: Int): Boolean
    fun updateGlobalData(data: CustomGlobalData)
}

class EngineImpl(private val nm: NetworkManager, private var internalGlobalData: GlobalData, private val program: Aggregate<Int>.(SensorData) -> NodeState) : Engine {

    override val globalData: GlobalData get() = internalGlobalData
    private var currentSensing: MutableMap<Int, SensorData> = mutableMapOf()
    private val nodes: List<Collektive<Int, NodeState>> = (0 until globalData.totalNodes).map { id ->
        val network = Network(id, nm)
        Collektive(id, network) {
            val sensorData = currentSensing[id]
            require(sensorData != null) { "Sensor data should never be null here" }
            return@Collektive program(sensorData)
        }
    }

    override fun step(id: Int, sensorData: SensorData): NodeState {
        currentSensing[id] = sensorData
        return nodes.first { it.localId == id }.cycle()
    }

    override fun addConnection(node1: Int, node2: Int): Boolean = nm.addConnection(node1, node2)

    override fun removeConnection(node1: Int, node2: Int): Boolean = nm.removeConnection(node1, node2)

    override fun updateGlobalData(data: CustomGlobalData) { internalGlobalData = internalGlobalData.copy(customData = data) }
}