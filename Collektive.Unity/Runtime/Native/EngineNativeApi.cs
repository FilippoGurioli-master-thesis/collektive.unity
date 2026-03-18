using System;
using System.Runtime.InteropServices;
using Collektive.Unity.Schema;
using Google.Protobuf;

namespace Collektive.Unity.Native
{
    public class EngineNativeApi : IEngine
    {
        private const string LibName = "collektive_backend";

        [DllImport(LibName, EntryPoint = "initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void InternalInitialize();

        [DllImport(LibName, EntryPoint = "step", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Step(
            int id,
            byte[] sensorData,
            int dataSize,
            out int outputSize
        );

        [DllImport(LibName, EntryPoint = "subscribe", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool InternalSubscribe(int node1, int node2);

        [DllImport(
            LibName,
            EntryPoint = "unsubscribe",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern bool InternalUnsubscribe(int node1, int node2);

        [DllImport(LibName, EntryPoint = "add_node", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool InternalAddNode(int id);

        [DllImport(
            LibName,
            EntryPoint = "remove_node",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern bool InternalRemoveNode(int id);

        [DllImport(
            LibName,
            EntryPoint = "free_result",
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void FreeResult(IntPtr pointer);

        public bool Subscribe(int node1, int node2) => InternalSubscribe(node1, node2);

        public bool Unsubscribe(int node1, int node2) => InternalUnsubscribe(node1, node2);

        public bool AddNode(int id) => InternalAddNode(id);

        public bool RemoveNode(int id) => InternalRemoveNode(id);

        public void Initialize() => InternalInitialize();

        public ActuatorData Step(int id, SensorData sensingData)
        {
            var encodedSensing = sensingData.ToByteArray();
            var resultPtr = Step(id, encodedSensing, encodedSensing.Length, out int outputSize);
            if (resultPtr == IntPtr.Zero)
                return null;
            try
            {
                var managedBuffer = new byte[outputSize];
                Marshal.Copy(resultPtr, managedBuffer, 0, outputSize);
                return ActuatorData.Parser.ParseFrom(managedBuffer);
            }
            finally
            {
                FreeResult(resultPtr);
            }
        }
    }
}
