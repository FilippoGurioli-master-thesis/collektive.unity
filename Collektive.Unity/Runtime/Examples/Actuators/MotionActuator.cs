using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity.Examples.Actuators
{
    [RequireComponent(typeof(Rigidbody))]
    public class MotionActuator : MonoBehaviour, IActuator
    {
        [SerializeField]
        private SimulationSettings globalData;

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Act(ActuatorData data)
        {
            var targetPosition = new Vector3(
                data.TargetPosition.X,
                data.TargetPosition.Y,
                data.TargetPosition.Z
            );
            Vector3 direction = targetPosition - transform.position;
            Vector3 normalizedDirection = direction.normalized;
            rb.AddForce(normalizedDirection * globalData.ForceMagnitude);
        }
    }
}
