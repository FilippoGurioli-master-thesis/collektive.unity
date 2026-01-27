package it.unibo.collektive.unity.examples

import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.aggregate.api.neighboring
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData

fun Aggregate<Int>.entrypoint(sensorData: SensorData): NodeState =
    NodeState(
        neighboring(sensorData)
            .neighbors.sequence
            .filter { it.value.sourceIntensity > sensorData.sourceIntensity }
            .maxByOrNull { it.value.sourceIntensity }?.value
            ?.currentPosition ?: sensorData.currentPosition
    )
