using System;
using UnityEngine;

namespace Collektive.Unity.Core.Neighborhood
{
    [RequireComponent(typeof(AgentComponent))]
    public abstract class Neighborhood : MonoBehaviour
    {
        private AgentComponent _agentComponent;

        public event Action<Agent> OnSubscribe;
        public event Action<Agent> OnUnsubscribe;

        private void Awake()
        {
            _agentComponent = GetComponent<AgentComponent>();
        }

        protected void SubscribeTo(Agent other)
        {
            _agentComponent.Agent.SubscribeTo(other.Id);
            OnSubscribe?.Invoke(other);
        }

        protected void UnsubscribeFrom(Agent other)
        {
            _agentComponent.Agent.UnsubscribeFrom(other.Id);
            OnUnsubscribe?.Invoke(other);
        }
    }
}
