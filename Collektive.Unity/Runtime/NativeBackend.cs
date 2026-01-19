using System;
using System.Runtime.InteropServices;
using Collektive.Unity.Generated;
using Google.Protobuf;

namespace Collektive.Unity
{
    public static class NativeBackend
    {
        private const string LibraryName = "collektive_backend";

        [DllImport(
            LibraryName,
            EntryPoint = "process_dinosaur",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern int ProcessDinosaur(IntPtr data, int length);

        public static int ProcessDinosaur(Dinosaur dinosaur)
        {
            byte[] bytes = dinosaur.ToByteArray();
            unsafe
            {
                fixed (byte* p = bytes)
                {
                    return NativeBackend.ProcessDinosaur((IntPtr)p, bytes.Length);
                }
            }
        }
    }
}
