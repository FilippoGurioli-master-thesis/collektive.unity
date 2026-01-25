using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Collektive.Unity.Data;
using Collektive.Unity.Schema;
using Google.Protobuf;

namespace Collektive.Unity.Native
{
    public static class EngineNativeApi
    {
        private const string LibName = "collektive_backend";

        [DllImport(LibName, EntryPoint = "initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize(IntPtr dataPointer, int dataSize);

        [DllImport(LibName, EntryPoint = "step", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Step(
            int id,
            byte[] sensorData,
            int dataSize,
            out int outputSize
        );

        [DllImport(
            LibName,
            EntryPoint = "add_connection",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool AddConnection(int node1, int node2);

        [DllImport(
            LibName,
            EntryPoint = "remove_connection",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool RemoveConnection(int node1, int node2);

        [DllImport(LibName, EntryPoint = "add_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddNode(int id);

        [DllImport(
            LibName,
            EntryPoint = "remove_node",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool RemoveNode(int id);

        [DllImport(
            LibName,
            EntryPoint = "update_global_data",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void UpdateGlobalData(IntPtr dataPointer, int dataSize);

        [DllImport(
            LibName,
            EntryPoint = "free_result",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void FreeResult(IntPtr pointer);

        public static void Initialize(GlobalData globalData)
        {
            var rawData = globalData.ToByteArray();
            var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var pointer = handle.AddrOfPinnedObject();
                Initialize(pointer, rawData.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        public static NodeState Step(int id, SensorData sensingData)
        {
            var encodedSensing = sensingData.ToByteArray();
            var resultPtr = Step(id, encodedSensing, encodedSensing.Length, out int outputSize);
            if (resultPtr == IntPtr.Zero)
                return null;
            try
            {
                var managedBuffer = new byte[outputSize];
                Marshal.Copy(resultPtr, managedBuffer, 0, outputSize);
                return NodeState.Parser.ParseFrom(managedBuffer);
            }
            finally
            {
                FreeResult(resultPtr);
            }
        }

        public static void UpdateGlobalData(CustomGlobalData data)
        {
            var rawData = data.ToByteArray();
            var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var pointer = handle.AddrOfPinnedObject();
                UpdateGlobalData(pointer, rawData.Length);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
