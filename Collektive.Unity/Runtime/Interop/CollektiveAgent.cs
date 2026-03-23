using System;
using System.Runtime.InteropServices;
using Collektive.Unity.Schema;
using Google.Protobuf;
using UnityEngine;

namespace Collektive.Unity.Interop
{
    public class CollektiveAgent : IDisposable
    {
        public int Id { get; private set; }

        public CollektiveAgent(int id)
        {
            Id = id;
            if (!CollektiveBindings.InternalAddNode(Id))
                throw new InvalidOperationException(
                    $"Collektive native has rejected the addition of {Id}"
                );
        }

        public bool SubscribeTo(int otherNode) =>
            CollektiveBindings.InternalSubscribe(Id, otherNode);

        public bool SubscribeTo(CollektiveAgent other) => SubscribeTo(other.Id);

        public bool UnsubscribeFrom(int otherNode) =>
            CollektiveBindings.InternalUnsubscribe(Id, otherNode);

        public bool UnsubscribeFrom(CollektiveAgent other) => UnsubscribeFrom(other.Id);

        public void Dispose()
        {
            if (!CollektiveBindings.InternalRemoveNode(Id))
                Debug.LogWarning($"Collektive native has rejected the remotion of {Id}");
        }

        public ActuatorData Compute(SensorData sensingData)
        {
            var encodedSensing = sensingData.ToByteArray();
            var resultPtr = CollektiveBindings.Step(
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
                CollektiveBindings.FreeResult(resultPtr);
            }
        }
    }
}
