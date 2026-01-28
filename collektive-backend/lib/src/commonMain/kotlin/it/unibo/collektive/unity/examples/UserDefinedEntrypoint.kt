package it.unibo.collektive.unity.examples

import it.unibo.collektive.aggregate.api.Aggregate
import it.unibo.collektive.aggregate.api.neighboring
import it.unibo.collektive.unity.schema.NodeState
import it.unibo.collektive.unity.schema.SensorData
import it.unibo.collektive.unity.shared.Vector3Helper
import it.unibo.collektive.unity.shared.div
import it.unibo.collektive.unity.shared.minus
import it.unibo.collektive.unity.shared.plus
import it.unibo.collektive.unity.shared.times
import kotlin.math.*

fun Aggregate<Int>.entrypoint(sensorData: SensorData): NodeState {
  val neighbors = neighboring(sensorData).neighbors.sequence
  val gradientDirection =
          neighbors.fold(Vector3Helper.zero()) { acc, nbr ->
            val relativeVector = nbr.value.currentPosition - sensorData.currentPosition
            val intensityDiff = nbr.value.sourceIntensity - sensorData.sourceIntensity
            acc + (relativeVector * intensityDiff.toFloat())
          }
  return if (gradientDirection == Vector3Helper.zero()) {
    NodeState(sensorData.currentPosition)
  } else {
    NodeState(sensorData.currentPosition + gradientDirection)
  }
}
    // NodeState(
    //     neighboring(sensorData)
    //         .neighbors.sequence
    //         .filter { it.value.sourceIntensity > sensorData.sourceIntensity }
    //         .maxByOrNull { it.value.sourceIntensity }?.value
    //         ?.currentPosition ?: sensorData.currentPosition
    // )
