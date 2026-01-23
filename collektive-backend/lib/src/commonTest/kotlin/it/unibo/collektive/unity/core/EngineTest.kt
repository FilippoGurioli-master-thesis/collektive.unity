package it.unibo.collektive.unity.core

import it.unibo.collektive.networking.NeighborsData
import it.unibo.collektive.networking.NoNeighborsData
import it.unibo.collektive.networking.OutboundEnvelope
import it.unibo.collektive.unity.core.network.NetworkManager
import it.unibo.collektive.unity.data.GlobalData
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

fun noOp(): NodeState = NodeState()

var counter = 0

fun sideEffectOp(): NodeState {
    counter += 1
    return NodeState()
}

class EngineTest {

    private fun createSensor() = SensorData()

    @Test
    fun testAddAndRemoveNode() {
        val nm = FakeNetworkManager()
        val engine = EngineImpl(nm, GlobalData(2.0)) { noOp() }
        assertTrue(engine.addNode(0), "Node 0 should be added")
        assertTrue(!engine.addNode(0), "Should not be able to add the same ID twice")
        assertTrue(engine.removeNode(0), "Node 0 should be removed")
        assertTrue(!engine.removeNode(0), "Node 0 should already be gone")
    }

    @Test
    fun testEngineInitialization() {
        val nm = FakeNetworkManager()
        val engine = EngineImpl(nm, GlobalData(deltaTime = 0.1)) { noOp() }
        engine.addNode(0)
        assertTrue(nm.hasRegistered)
    }

    @Test
    fun testStepProcessing() {
        val nm = FakeNetworkManager()
        val globalData = GlobalData(deltaTime = 0.02)
        val engine = EngineImpl(nm, globalData) { sideEffectOp() }
        engine.addNode(0)
        engine.addNode(1)
        val sensing0 = createSensor()
        val sensing1 = createSensor()
        engine.step(0, sensing0)
        engine.step(1, sensing1)
        assertEquals(counter, 2, "Should have been cycled twice")
        assertTrue(nm.hasSent || nm.hasReceived, "Network should have been engaged during the cycle")
    }

    @Test
    fun testConnectionManagement() {
        val nm = FakeNetworkManager()
        val engine = EngineImpl(nm, GlobalData(deltaTime = 1.0)) { noOp() }
        engine.addNode(0)
        engine.addNode(1)
        assertTrue(engine.addConnection(0, 1), "Connection should be added")
        assertTrue(nm.connections.contains(0 to 1), "NetworkManager should reflect the change")
        assertTrue(engine.removeConnection(0, 1), "Connection should be removed")
        assertEquals(0, nm.connections.size)
    }
}
