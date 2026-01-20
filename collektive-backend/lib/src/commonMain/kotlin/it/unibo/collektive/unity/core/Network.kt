package it.unibo.collektive.unity.core

import it.unibo.collektive.aggregate.api.DataSharingMethod
import it.unibo.collektive.networking.Mailbox
import it.unibo.collektive.networking.Message
import it.unibo.collektive.networking.NeighborsData
import it.unibo.collektive.networking.NoNeighborsData
import it.unibo.collektive.networking.OutboundEnvelope
import it.unibo.collektive.path.Path

class Network(val id: Int, val nm: NetworkManager) : Mailbox<Int> {

    override val inMemory: Boolean = false

    init { nm.registerNode(id) }

    override fun deliverableFor(outboundMessage: OutboundEnvelope<Int>) = nm.send(id, outboundMessage)

    override fun deliverableReceived(message: Message<Int, *>) =
        error("This network is supposed to be in-memory, no need to deliver messages since it is already in the buffer")

    override fun currentInbound(): NeighborsData<Int> = nm.receiveMessageFor(id)
}

/**
 * Network manager that handles messaging.
 */
interface NetworkManager {

    /**
     * Registers a node in the global network.
     */
    fun registerNode(id: Int)

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
    private val messageBuffer = mutableMapOf<Int, Message<Int, *>>()

    override fun registerNode(id: Int) {
        adjacencyMap.getOrPut(id) { mutableSetOf() }
    }

    override fun send(local: Int, envelope: OutboundEnvelope<Int>) {
        adjacencyMap[local]?.forEach {
           val message = envelope.prepareMessageFor(local)
            val inboundForNeighbor = messageBuffer.getOrPut(it) { mutableMapOf() }
            inboundForNeighbor[local] = message
        }
    }

    override fun receiveMessageFor(id: Int): NeighborsData<Int> {
        val neighbors = adjacencyMap[id] ?: return NoNeighborsData()
        return NeighborsDataImpl(
            neighbors,
            messageBuffer
        )
    }

    override fun removeConnection(node1: Int, node2: Int): Boolean {
        val r1 = adjacencyMap[node1]?.remove(node2) ?: false
        val r2 = adjacencyMap[node2]?.remove(node1) ?: false
        return r1 || r2
    }

    override fun addConnection(node1: Int, node2: Int): Boolean {
        val a1 = adjacencyMap.getOrPut(node1) { mutableSetOf() }.add(node2)
        val a2 = adjacencyMap.getOrPut(node2) { mutableSetOf() }.add(node1)
        return a1 || a2
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
