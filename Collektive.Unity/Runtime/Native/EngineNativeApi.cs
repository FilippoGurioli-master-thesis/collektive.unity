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
        private static extern IntPtr Step(IntPtr[] pointers, int[] sizes, [Out] int[] outputSizes);

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

        [DllImport(
            LibName,
            EntryPoint = "update_global_data",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void UpdateGlobalData(IntPtr dataPointer, int dataSize);

        [DllImport(
            LibName,
            EntryPoint = "free_results",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void FreeResults(IntPtr pointers, int count);

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

        public static List<NodeState> Step(List<SensorData> sensingData)
        {
            var count = sensingData.Count;
            var pointers = new IntPtr[count];
            var sizes = new int[count];
            var outputSizes = new int[count];
            var handles = new GCHandle[count];
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var bytes = sensingData[i].ToByteArray();
                    handles[i] = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                    pointers[i] = handles[i].AddrOfPinnedObject();
                    sizes[i] = bytes.Length;
                }
                var resultPtrArray = Step(pointers, sizes, outputSizes);
                if (resultPtrArray == IntPtr.Zero)
                    throw new InvalidOperationException("Result returned by the backend was null");
                var nodeDataPtrs = new IntPtr[count];
                Marshal.Copy(resultPtrArray, nodeDataPtrs, 0, count);
                var results = new List<NodeState>();
                for (var i = 0; i < count; i++)
                {
                    var managedBytes = new byte[outputSizes[i]];
                    Marshal.Copy(nodeDataPtrs[i], managedBytes, 0, outputSizes[i]);
                    results.Add(NodeState.Parser.ParseFrom(managedBytes));
                }
                FreeResults(resultPtrArray, count);
                return results;
            }
            finally
            {
                foreach (var h in handles)
                    if (h.IsAllocated)
                        h.Free();
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
