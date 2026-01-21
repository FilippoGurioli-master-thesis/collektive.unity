using Collektive.Unity.Attributes;
using Collektive.Unity.Schema;
using Collektive.Unity.Shared;
using UnityEngine;

namespace Collektive.Unity.Example
{
    [RequireComponent(typeof(Renderer))]
    public class GradientNode : Node
    {
        [SerializeField]
        private bool isSource;

        [SerializeField, ReadOnly]
        private double gradient;

        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        public override SensorData Sense() =>
            new SensorData
            {
                Position = new Shared.Vector3
                {
                    X = transform.position.x,
                    Y = transform.position.y,
                    Z = transform.position.z,
                },
                IsSource = isSource,
            };

        protected override void Act(NodeState state)
        {
            gradient = state.Gradient;
            var t = Mathf.InverseLerp(0f, 30f, (float)gradient);
            var color = isSource ? Color.white : Color.HSVToRGB(t, 1f, 1f);
            _renderer.material.color = color;
        }
    }
}
