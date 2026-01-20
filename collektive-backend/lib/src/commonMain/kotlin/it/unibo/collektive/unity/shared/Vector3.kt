package it.unibo.collektive.unity.shared

import kotlin.math.pow
import kotlin.math.sqrt

/**
 * Computes the geometric distance between this vector and the passed one.
 */
fun Vector3.distanceTo(other: Vector3): Float =
    sqrt(
        (this.x - other.x).pow(2) + (this.y - other.y).pow(2) + (this.z - other.z).pow(2)
    )

/**
 * Computes the geometric distance between the passed vectors.
 */
object Vector3Helper {
    fun distance(a: Vector3, b: Vector3) =
        a.distanceTo(b)
}
