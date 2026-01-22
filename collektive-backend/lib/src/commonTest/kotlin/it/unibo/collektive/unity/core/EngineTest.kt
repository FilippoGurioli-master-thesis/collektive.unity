package it.unibo.collektive.unity.core

import it.unibo.collektive.Collektive
import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.aggregate.api.share
import it.unibo.collektive.networking.NeighborsData
import it.unibo.collektive.networking.NoNeighborsData
import it.unibo.collektive.networking.OutboundEnvelope
import it.unibo.collektive.unity.core.network.Network
import it.unibo.collektive.unity.core.network.NetworkManager
import it.unibo.collektive.unity.data.GlobalData
import it.unibo.collektive.unity.examples.entrypoint
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertTrue

class FakeNetworkManager : NetworkManager {
    var hasRegistered: Boolean = false
    var hasSent: Boolean = false
    var hasReceived: Boolean = false
    val connections = mutableSetOf<Pair<Int, Int>>()
    override fun addConnection(node1: Int, node2: Int): Boolean = connections.add(node1 to node2)
    override fun removeConnection(node1: Int, node2: Int): Boolean = connections.remove(node1 to node2)
    override fun registerNode(id: Int) {
        hasRegistered = true
    }

    override fun unregisterNode(id: Int) {
    }

    override fun send(local: Int, envelope: OutboundEnvelope<Int>) {
        hasSent = true
    }

    override fun receiveMessageFor(id: Int): NeighborsData<Int>
    {
        hasReceived = true
        return NoNeighborsData()
    }
}

class EngineTest {

    private fun createSensor() = SensorData()

    @Test
    fun testEngineInitialization() {
        val nm = FakeNetworkManager()
        val totalNodes = 3
        val globalData = GlobalData(totalNodes = totalNodes, deltaTime = 0.1)
        EngineImpl(nm, globalData)
        assertTrue(nm.hasRegistered)
    }

    @Test
    fun testStepProcessing() {
        val nm = FakeNetworkManager()
        val totalNodes = 2
        val globalData = GlobalData(totalNodes = totalNodes, deltaTime = 0.02)
        val engine = EngineImpl(nm, globalData)
        val sensing = listOf(createSensor(), createSensor())
        val results = engine.step(sensing)
        assertEquals(totalNodes, results.size)
        assertTrue(nm.hasSent && nm.hasReceived)
    }

    @Test
    fun testConnectionManagement() {
        val nm = FakeNetworkManager()
        val engine = EngineImpl(nm, GlobalData(totalNodes = 2, deltaTime = 1.0))
        assertTrue(engine.addConnection(0, 1), "Connection should be added")
        assertTrue(nm.connections.contains(0 to 1), "NetworkManager should reflect the change")
        assertTrue(engine.removeConnection(0, 1), "Connection should be removed")
        assertEquals(0, nm.connections.size)
    }
}
