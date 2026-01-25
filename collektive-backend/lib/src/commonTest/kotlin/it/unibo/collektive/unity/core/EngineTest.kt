package it.unibo.collektive.unity.core

import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.networking.NeighborsData
import it.unibo.collektive.networking.NoNeighborsData
import it.unibo.collektive.networking.OutboundEnvelope
import it.unibo.collektive.unity.core.network.NetworkManager
import it.unibo.collektive.unity.data.GlobalData
import it.unibo.collektive.unity.schema.CustomGlobalData
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import kotlin.test.BeforeTest
import kotlin.test.Test
import kotlin.test.assertFalse
import kotlin.test.assertTrue

class EngineTest {

    private lateinit var nm: MockNetworkManager
    private lateinit var engine: EngineImpl
    private val initialGlobalData = GlobalData(42, CustomGlobalData())

    private var isProgramExecuted = false

    // Aggregate program that echoes the sensor value
    private val program: Aggregate<Int>.(SensorData) -> NodeState = { sensor ->
        isProgramExecuted = true
        NodeState()
    }

    class MockNetworkManager : NetworkManager {
        val registeredNodes: MutableList<Int> = mutableListOf()
        val messageSent: MutableList<Int> = mutableListOf()
        val messageReceived: MutableList<Int> = mutableListOf()
        var isConnectionAdd: Boolean = false
        var isConnectionRemove: Boolean = false

        override fun registerNode(id: Int) {
            registeredNodes.add(id)
        }

        override fun unregisterNode(id: Int) {
            registeredNodes.remove(id)
        }

        override fun send(local: Int, envelope: OutboundEnvelope<Int>) {
            messageSent.add(local)
        }

        override fun receiveMessageFor(id: Int): NeighborsData<Int> {
            messageReceived.add(id)
            return NoNeighborsData()
        }

        override fun removeConnection(node1: Int, node2: Int): Boolean {
            isConnectionRemove = true
            return true
        }

        override fun addConnection(node1: Int, node2: Int): Boolean {
            isConnectionAdd = true
            return true
        }

    }

    @BeforeTest
    fun setup() {
        nm = MockNetworkManager()
        engine = EngineImpl(nm, initialGlobalData, program)
        isProgramExecuted = false
    }

    @Test
    fun testAddNode() {
        assertTrue(engine.addNode(1), "Should successfully add a new node")
        assertTrue(nm.registeredNodes.contains(1), "Nm should contain node too")
        assertFalse(engine.addNode(1), "Should return false when adding a duplicate node ID")
    }

    @Test
    fun testRemoveNode() {
        val nodeId = 5
        engine.addNode(nodeId)
        assertTrue(engine.removeNode(nodeId), "Should successfully remove an existing node")
        assertFalse(nm.registeredNodes.contains(1), "Nm should not contain the node anymore too")
        assertFalse(engine.removeNode(nodeId), "Should return false when removing a non-existent node")
    }

    @Test
    fun testStepExecution() {
        val nodeId = 10
        val sensorData = SensorData()
        engine.addNode(nodeId)
        engine.step(nodeId, sensorData)
        assertTrue(isProgramExecuted)
    }

    @Test
    fun testConnectionDelegation() {
        assertTrue(engine.addConnection(1, 2))
        assertTrue(nm.isConnectionAdd)
        assertTrue(engine.removeConnection(1, 2))
        assertTrue(nm.isConnectionRemove)
    }
}
