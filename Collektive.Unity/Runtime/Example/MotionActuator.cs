using Collektive.Unity.CollectiveNode;
using Collektive.Unity.Globals;
using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity.Example
{
    [RequireComponent(typeof(Rigidbody))]
    public class MotionActuator : MonoBehaviour, IActuator
    {
        [SerializeField]
        private GlobalData globalData;

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
