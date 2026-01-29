using System;
using Collektive.Unity.Attributes;
using Collektive.Unity.Schema;
using UnityEngine;
using Random = System.Random;

namespace Collektive.Unity
{
    /// <summary>
    /// A collektive Node.
    /// </summary>
    public abstract class Node : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private int id;

        private Random _prng;
        private bool _isQuitting = false;

        public int Id
        {
            get => id;
            private set => id = value;
        }

        public Action<NodeState> OnStateReceived;

        public abstract SensorData Sense();
        protected abstract void Act(NodeState state);

        protected float GetNextRandom() => (float)_prng.NextDouble();

        protected float GetGaussian(float mean, float stdDev)
        {
            var u1 = 1.0 - _prng.NextDouble();
            var u2 = 1.0 - _prng.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * (float)randStdNormal;
        }

        private void OnEnable()
        {
            Id = SimulationManager.Instance.AddNode(this);
            name = $"node {Id}";
            _prng = new Random(SimulationManager.Instance.GlobalData.Seed + Id);
            OnStateReceived += Act;
            Application.wantsToQuit += OnQuit;
        }

        private void OnDisable()
        {
            if (!_isQuitting)
                SimulationManager.Instance.RemoveNode(this);
            OnStateReceived -= Act;
        }

        private bool OnQuit()
        {
            _isQuitting = true;
            return true;
        }
    }
}
