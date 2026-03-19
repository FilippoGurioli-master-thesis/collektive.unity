using System;
using System.Runtime.InteropServices;
using Collektive.Unity.Schema;
using Google.Protobuf;

namespace Collektive.Unity.BackendWrapper
{
    public static class CollektiveNative
    {
        private const string LibName = "collektive_backend";

        [DllImport(LibName, EntryPoint = "initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InternalInitialize();

        [DllImport(LibName, EntryPoint = "step", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Step(
            int id,
            byte[] sensorData,
            int dataSize,
            out int outputSize
        );

        [DllImport(LibName, EntryPoint = "subscribe", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InternalSubscribe(int node1, int node2);

        [DllImport(
            LibName,
            EntryPoint = "unsubscribe",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool InternalUnsubscribe(int node1, int node2);

        [DllImport(LibName, EntryPoint = "add_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InternalAddNode(int id);

        [DllImport(
            LibName,
            EntryPoint = "remove_node",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool InternalRemoveNode(int id);

        [DllImport(
            LibName,
            EntryPoint = "free_result",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern void FreeResult(IntPtr pointer);
    }
}
