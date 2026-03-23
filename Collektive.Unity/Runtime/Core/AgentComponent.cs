using Collektive.Unity.Attributes;
using Collektive.Unity.BackendWrapper;
using UnityEngine;

namespace Collektive.Unity.Core
{
    [DisallowMultipleComponent]
    public class AgentComponent : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private int id;

        public CollektiveAgent Agent { get; private set; }

        private void OnEnable()
        {
            Agent = new CollektiveAgent(GetInstanceID());
            id = Agent.Id;
            name = $"node {id}";
        }

        private void OnDisable() => Agent?.Dispose();
    }
}
