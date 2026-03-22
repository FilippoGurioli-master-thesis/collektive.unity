using Collektive.Unity.CollectiveNode;
using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity.Example
{
    public class PositionSensor : MonoBehaviour, ISensor
    {
        public void Contribute(SensorData data)
        {
            data.CurrentPosition = new Shared.Vector3
            {
                X = transform.position.x,
                Y = transform.position.y,
                Z = transform.position.z,
            };
        }
    }
}
