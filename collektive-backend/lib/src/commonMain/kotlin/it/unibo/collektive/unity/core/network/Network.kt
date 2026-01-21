package it.unibo.collektive.unity.core.network

import it.unibo.collektive.networking.Mailbox
import it.unibo.collektive.networking.Message
import it.unibo.collektive.networking.NeighborsData
import it.unibo.collektive.networking.OutboundEnvelope

class Network(val id: Int, val nm: NetworkManager) : Mailbox<Int> {

    override val inMemory: Boolean = false

    init { nm.registerNode(id) }

    override fun deliverableFor(outboundMessage: OutboundEnvelope<Int>) = nm.send(id, outboundMessage)

    override fun deliverableReceived(message: Message<Int, *>) =
        error("This network is supposed to be in-memory, no need to deliver messages since it is already in the buffer")

    override fun currentInbound(): NeighborsData<Int> = nm.receiveMessageFor(id)
}
