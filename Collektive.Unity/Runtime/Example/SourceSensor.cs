using Collektive.Unity.Attributes;
using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity.Example
{
    [RequireComponent(typeof(Rigidbody), typeof(ObstacleSensor))]
    public class SourceSensor : Node
    {
        [Header("Input")]
        [SerializeField]
        private Transform source;

        [SerializeField]
        private float maxSpeed = 5;

        [SerializeField]
        private float steeringForce = 10;

        [SerializeField]
        private float intensity = 100;

        [SerializeField]
        private float sigma = 10;

        [Header("Output")]
        [SerializeField, ReadOnly]
        private float sourceIntensity;

        [SerializeField, ReadOnly]
        private Vector3 targetPosition;

        private Rigidbody _rb;
        private ObstacleSensor _obstacleSensor;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _obstacleSensor = GetComponent<ObstacleSensor>();
        }

        public override SensorData Sense()
        {
            var data = new SensorData
            {
                SourceIntensity = SenseField(),
                CurrentPosition = new Shared.Vector3
                {
                    X = transform.position.x,
                    Y = transform.position.y,
                    Z = transform.position.z,
                },
            };
            foreach (var obstacle in _obstacleSensor.Sense())
            {
                data.Obstacles.Add(
                    new Shared.Vector3
                    {
                        X = obstacle.x,
                        Y = obstacle.y,
                        Z = obstacle.z,
                    }
                );
            }

            return data;
        }

        // gaussian model V(d) = I * e^(-d^2/(2 * sigma^2))
        private float SenseField()
        {
            var distance = Vector3.Distance(transform.position, source.position);
            sourceIntensity =
                intensity * Mathf.Exp(-Mathf.Pow(distance, 2) / (2 * Mathf.Pow(sigma, 2)));
            return sourceIntensity;
        }

        protected override void Act(NodeState state)
        {
            Move(
                new Vector3(state.TargetPosition.X, state.TargetPosition.Y, state.TargetPosition.Z)
            );
        }

        private void Move(Vector3 targetPosition)
        {
            var direction = targetPosition - transform.position;
            this.targetPosition = targetPosition;
            if (direction == Vector3.zero)
                return;
            var desiredVelocity = direction.normalized * maxSpeed;
            var steering = desiredVelocity - _rb.linearVelocity;
            _rb.AddForce(Vector3.ClampMagnitude(steering, steeringForce), ForceMode.Force);
        }
    }
}
