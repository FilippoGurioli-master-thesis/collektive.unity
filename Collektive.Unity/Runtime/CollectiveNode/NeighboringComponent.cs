using System;
using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(CollektiveNodeComponent))]
    public abstract class NeighboringComponent : MonoBehaviour
    {
        private CollektiveNodeComponent _nodeComponent;

        public event Action<Node> OnSubscribe;
        public event Action<Node> OnUnsubscribe;

        private void Awake()
        {
            _nodeComponent = GetComponent<CollektiveNodeComponent>();
        }

        protected void SubscribeTo(Node other)
        {
            _nodeComponent.Node.SubscribeTo(other.Id);
            OnSubscribe?.Invoke(other);
        }

        protected void UnsubscribeFrom(Node other)
        {
            _nodeComponent.Node.UnsubscribeFrom(other.Id);
            OnUnsubscribe?.Invoke(other);
        }
    }
}
