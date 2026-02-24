package it.unibo.collektive.unity.core

import kotlinx.cinterop.ExperimentalForeignApi
import kotlinx.cinterop.alloc
import kotlinx.cinterop.nativeHeap
import kotlinx.cinterop.ptr
import platform.posix.pthread_mutex_init
import platform.posix.pthread_mutex_lock
import platform.posix.pthread_mutex_t
import platform.posix.pthread_mutex_unlock

@OptIn(ExperimentalForeignApi::class)
actual class Lock {
    private val mutex = nativeHeap.alloc<pthread_mutex_t>()

    init {
        pthread_mutex_init(mutex.ptr, null)
    }

    actual fun <T> withLock(block: () -> T): T {
        pthread_mutex_lock(mutex.ptr)
        try { return block() } finally {
            pthread_mutex_unlock(mutex.ptr)
        }
    }
}
