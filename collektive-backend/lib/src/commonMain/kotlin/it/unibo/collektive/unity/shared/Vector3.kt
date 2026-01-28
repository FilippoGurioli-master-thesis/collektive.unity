package it.unibo.collektive.unity.shared

import kotlin.math.pow
import kotlin.math.sqrt

/** Computes the geometric distance between this vector and the passed one. */
fun Vector3.distanceTo(other: Vector3): Float =
        sqrt((this.x - other.x).pow(2) + (this.y - other.y).pow(2) + (this.z - other.z).pow(2))

/** Computes the normalized version of this vector. */
fun Vector3.normalized(): Vector3 {
  val magnitude = this.magnitude()
  if (magnitude < 1e-6) return Vector3Helper.zero()
  return Vector3(this.x / magnitude, this.y / magnitude, this.z / magnitude)
}

/** Computes the magnitude of this vector. */
fun Vector3.magnitude(): Float = this.distanceTo(Vector3Helper.zero())

/** Computes the sum of this vector and the passed one. */
operator fun Vector3?.plus(other: Vector3?): Vector3 =
        Vector3(
                (this?.x ?: 0f) + (other?.x ?: 0f),
                (this?.y ?: 0f) + (other?.y ?: 0f),
                (this?.z ?: 0f) + (other?.z ?: 0f)
        )

/** Computes the difference between this vector and the passed one. */
operator fun Vector3?.minus(other: Vector3?): Vector3 =
        Vector3(
                (this?.x ?: 0f) - (other?.x ?: 0f),
                (this?.y ?: 0f) - (other?.y ?: 0f),
                (this?.z ?: 0f) - (other?.z ?: 0f)
        )

/** Computes the scalar multiplication of the vector. */
operator fun Vector3?.times(scalar: Float): Vector3 =
        Vector3((this?.x ?: 0f) * scalar, (this?.y ?: 0f) * scalar, (this?.z ?: 0f) * scalar)

/** Divides the vector by a scalar (needed for your average calculation). */
operator fun Vector3?.div(scalar: Float): Vector3 =
        Vector3((this?.x ?: 0f) / scalar, (this?.y ?: 0f) / scalar, (this?.z ?: 0f) / scalar)

object Vector3Helper {
  /** Computes the geometric distance between the passed vectors. */
  fun distance(a: Vector3, b: Vector3) = a.distanceTo(b)

  fun zero() = Vector3(0f, 0f, 0f)
}
