using Collektive.Unity.Attributes;
using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity.Examples.Sensors
{
    public class SourceSensor : MonoBehaviour, ISensor
    {
        [SerializeField]
        private Transform source;

        [SerializeField, ReadOnly]
        private double sensedData;

        public void Contribute(SensorData data)
        {
            data.SourceIntensity = (source.position - transform.position).magnitude;
            sensedData = data.SourceIntensity;
        }
    }
}
