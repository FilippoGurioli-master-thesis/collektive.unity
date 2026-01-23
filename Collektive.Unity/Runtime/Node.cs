using System;
using Collektive.Unity.Attributes;
using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity
{
    /// <summary>
    /// A collektive Node.
    /// </summary>
    public abstract class Node : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private int id;

        public int Id
        {
            get => id;
            set => id = value;
        }

        public Action<NodeState> OnStateReceived;

        public abstract SensorData Sense();
        protected abstract void Act(NodeState state);

        private void OnEnable() => OnStateReceived += Act;

        private void OnDisable() => OnStateReceived -= Act;
    }
}
