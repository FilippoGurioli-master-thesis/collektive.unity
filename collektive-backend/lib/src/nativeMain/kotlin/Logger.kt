//package it.unibo.collektive.unity.core

//import kotlinx.cinterop.*
//import kotlin.experimental.ExperimentalNativeApi

//@OptIn(ExperimentalForeignApi::class)
//private var logCallback: CPointer<CFunction<(CPointer<ByteVarOf<Byte>>?) -> Unit>>? = null
//
//@OptIn(ExperimentalForeignApi::class, ExperimentalNativeApi::class)
//@CName("set_log_callback")
//fun setLogCallback(callback: CPointer<CFunction<(CPointer<ByteVarOf<Byte>>?) -> Unit>>) {
    //logCallback = callback
//}
//
//@OptIn(ExperimentalForeignApi::class)
//fun log(message: String) {
    //logCallback?.let {
        //memScoped {
            //val cString = message.cstr.ptr
            //it.invoke(cString)
        //}
    //}
//}
