package it.unibo.collektive.unity.examples

import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.aggregate.api.share
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import it.unibo.collektive.unity.shared.Vector3
import it.unibo.collektive.unity.shared.distanceTo
import kotlin.Double.Companion.POSITIVE_INFINITY

fun Aggregate<Int>.entrypoint(sensorData: SensorData): NodeState =
    share(NodeState(POSITIVE_INFINITY, sensorData.position)) { field ->
        if (sensorData.isSource) return@share NodeState(0.0, sensorData.position)
        val best = field.neighbors
            .list.minOfOrNull { it.value.gradient + (sensorData.position?.distanceTo(it.value.position ?: Vector3()) ?: 0.0).toDouble() }
        NodeState(best ?: POSITIVE_INFINITY, sensorData.position)
    }
