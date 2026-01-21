package it.unibo.collektive.unity.core

import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertTrue

class LockTest {

    @Test
    fun testLockExecutesBlock() {
        val lock = Lock()
        var executed = false
        lock.withLock {
            executed = true
        }
        assertTrue(executed, "The code block inside withLock should be executed")
    }

    @Test
    fun testLockReturnsValue() {
        val lock = Lock()
        val expectedValue = "Hello Lock"
        val result = lock.withLock {
            expectedValue
        }
        assertEquals(expectedValue, result, "withLock should return the result of the block")
    }
}
