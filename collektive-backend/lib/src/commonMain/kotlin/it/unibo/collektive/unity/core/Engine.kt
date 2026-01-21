package it.unibo.collektive.unity.core

import it.unibo.collektive.Collektive
import it.unibo.collektive.unity.core.network.Network
import it.unibo.collektive.unity.core.network.NetworkManager
import it.unibo.collektive.unity.data.GlobalData
import it.unibo.collektive.unity.examples.entrypoint
import it.unibo.collektive.unity.schema.CustomGlobalData
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData

interface Engine {
    fun step(sensing: List<SensorData>): List<NodeState>
    fun addConnection(node1: Int, node2: Int): Boolean
    fun removeConnection(node1: Int, node2: Int): Boolean
    fun updateGlobalData(data: CustomGlobalData)
}

class EngineImpl(private val nm: NetworkManager, private var globalData: GlobalData) : Engine {

    private var currentSensing: List<SensorData>? = null
    private val nodes: List<Collektive<Int, NodeState>> = (0 until globalData.totalNodes).map { id ->
        val network = Network(id, nm)
        Collektive(id, network) {
            val sensorData = currentSensing?.get(id)
            require(sensorData != null) { "Sensor data should never be null here" }
            entrypoint(sensorData)
        }
    }

    override fun step(sensing: List<SensorData>): List<NodeState> {
        currentSensing = sensing
        return nodes.map { it.cycle() }
    }

    override fun addConnection(node1: Int, node2: Int): Boolean = nm.addConnection(node1, node2)

    override fun removeConnection(node1: Int, node2: Int): Boolean = nm.removeConnection(node1, node2)

    override fun updateGlobalData(data: CustomGlobalData) { globalData = globalData.copy(customData = data) }
}