package it.unibo.collektive.unity.core.network

import it.unibo.collektive.aggregate.api.DataSharingMethod
import it.unibo.collektive.networking.Message
import it.unibo.collektive.networking.NeighborsData
import it.unibo.collektive.networking.NoNeighborsData
import it.unibo.collektive.networking.OutboundEnvelope
import it.unibo.collektive.path.Path
import it.unibo.collektive.unity.core.Lock
import kotlin.collections.component1
import kotlin.collections.component2

/**
 * Network manager that handles messaging.
 */
interface NetworkManager {

    /**
     * Registers a node in the global network.
     */
    fun registerNode(id: Int)

    /**
     * Unregisters a node in the global network.
     */
    fun unregisterNode(id: Int)

    /**
     * Send the message from local id.
     */
    fun send(local: Int, envelope: OutboundEnvelope<Int>)

    /**
     * Retrieve message for id.
     */
    fun receiveMessageFor(id: Int): NeighborsData<Int>

    /**
     * Remove the connection between those 2 nodes.
     * @return true if the connection was successfully removed, false otherwise.
     */
    fun removeConnection(node1: Int, node2: Int): Boolean

    /**
     * Add the connection between those 2 nodes.
     * @return true if the connection was successfully added, false otherwise.
     */
    fun addConnection(node1: Int, node2: Int): Boolean
}

class NetworkManagerImpl : NetworkManager {

    private val adjacencyMap = mutableMapOf<Int, MutableSet<Int>>()
    private val outboxes = mutableMapOf<Int, OutboundEnvelope<Int>>()
    private val lock = Lock()

    override fun registerNode(id: Int) {
        adjacencyMap.getOrPut(id) { mutableSetOf() }
    }

    override fun unregisterNode(id: Int) {
        outboxes.remove(id)
        adjacencyMap.remove(id)
        adjacencyMap.values
            .filter { it.contains(id) }
            .forEach { it.remove(id) }
    }

    override fun send(local: Int, envelope: OutboundEnvelope<Int>) = lock.withLock {
        outboxes[local] = envelope
    }

    override fun receiveMessageFor(id: Int): NeighborsData<Int> = lock.withLock {
        val neighbors = adjacencyMap[id] ?: return@withLock NoNeighborsData()
        return@withLock NeighborsDataImpl(
            neighbors,
            neighbors.mapNotNull {nbrId ->
                outboxes[nbrId]?.let { envelope ->
                    nbrId to envelope.prepareMessageFor(id)
                }
            }.toMap()
        )
    }

    override fun removeConnection(node1: Int, node2: Int): Boolean = lock.withLock {
        val r1 = adjacencyMap[node1]?.remove(node2) ?: false
        val r2 = adjacencyMap[node2]?.remove(node1) ?: false
        return@withLock r1 || r2
    }

    override fun addConnection(node1: Int, node2: Int): Boolean = lock.withLock {
        val a1 = adjacencyMap.getOrPut(node1) { mutableSetOf() }.add(node2)
        val a2 = adjacencyMap.getOrPut(node2) { mutableSetOf() }.add(node1)
        return@withLock a1 || a2
    }
}

class NeighborsDataImpl(override val neighbors: Set<Int>, private val inbound: Map<Int, Message<Int, *>>) : NeighborsData<Int>
{
    @Suppress("UNCHECKED_CAST")
    override fun <Value> dataAt(
        path: Path,
        dataSharingMethod: DataSharingMethod<Value>
    ): Map<Int, Value> =
        inbound
            .asSequence()
            .filter { (senderId, _) -> senderId in neighbors }
            .mapNotNull { (senderId, msg) ->
                val raw = msg.sharedData.getOrElse(path) { NoValue } as Value
                if (raw == NoValue) null else senderId to raw
            }
            .toMap()

    private object NoValue
}
