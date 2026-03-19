using System;
using System.Runtime.InteropServices;
using Collektive.Unity.Schema;
using Google.Protobuf;
using UnityEngine;

namespace Collektive.Unity.BackendWrapper
{
    public class CollektiveNode : IDisposable
    {
        public int Id { get; private set; }

        public CollektiveNode(int id)
        {
            Id = id;
            if (!CollektiveNative.InternalAddNode(Id))
                throw new InvalidOperationException(
                    $"Collektive native has rejected the addition of {Id}"
                );
        }

        public bool SubscribeTo(int otherNode) => CollektiveNative.InternalSubscribe(Id, otherNode);

        public bool SubscribeTo(CollektiveNode other) => SubscribeTo(other.Id);

        public bool UnsubscribeFrom(int otherNode) =>
            CollektiveNative.InternalUnsubscribe(Id, otherNode);

        public bool UnsubscribeFrom(CollektiveNode other) => UnsubscribeFrom(other.Id);

        public bool ConnectTo(int otherNode)
        {
            var res1 = CollektiveNative.InternalSubscribe(Id, otherNode);
            var res2 = CollektiveNative.InternalSubscribe(otherNode, Id);
            return res1 && res2;
        }

        public bool ConnectTo(CollektiveNode other) => ConnectTo(other.Id);

        public bool DisconnectFrom(int otherNode)
        {
            var res1 = CollektiveNative.InternalUnsubscribe(Id, otherNode);
            var res2 = CollektiveNative.InternalUnsubscribe(otherNode, Id);
            return res1 && res2;
        }

        public bool DisconnectFrom(CollektiveNode other) => DisconnectFrom(other.Id);

        public void Dispose()
        {
            if (!CollektiveNative.InternalRemoveNode(Id))
                Debug.LogWarning($"Collektive native has rejected the remotion of {Id}");
        }

        public ActuatorData Compute(SensorData sensingData)
        {
            var encodedSensing = sensingData.ToByteArray();
            var resultPtr = CollektiveNative.Step(
                Id,
                encodedSensing,
                encodedSensing.Length,
                out int outputSize
            );
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
                CollektiveNative.FreeResult(resultPtr);
            }
        }
    }
}
