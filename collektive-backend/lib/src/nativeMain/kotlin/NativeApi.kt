import generated.Dinosaur
import kotlin.experimental.ExperimentalNativeApi
import kotlinx.cinterop.ByteVar
import kotlinx.cinterop.CPointer
import kotlinx.cinterop.ExperimentalForeignApi
import kotlinx.cinterop.readBytes

@OptIn(ExperimentalNativeApi::class, ExperimentalForeignApi::class)
@CName("process_dinosaur")
fun processDinosaur(data: CPointer<ByteVar>, length: Int): Int {
  val bytes = data.readBytes(length)
  val dinosaur = Dinosaur.ADAPTER.decode(bytes)
  println("[BACKEND] received: ${dinosaur.name}")
  return 18
}

