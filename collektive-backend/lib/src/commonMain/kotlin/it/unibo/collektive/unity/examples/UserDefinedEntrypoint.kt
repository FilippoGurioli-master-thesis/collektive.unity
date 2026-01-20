package it.unibo.collektive.unity.examples

import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.aggregate.api.share
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import kotlin.Double.Companion.POSITIVE_INFINITY

fun Aggregate<Int>.entrypoint(sensorData: SensorData): NodeState =
    share(NodeState(POSITIVE_INFINITY)) { field ->
        if (sensorData.isSource) return@share NodeState(0.0)
        val best = field.neighbors
            .list.minOfOrNull { it.value.gradient }
        NodeState(best ?: POSITIVE_INFINITY)
    }
