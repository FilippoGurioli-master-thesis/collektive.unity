using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(Node))]
    public abstract class NeighboringComponent : MonoBehaviour
    {
        private CollektiveNodeComponent _nodeComponent;

        private void Awake()
        {
            _nodeComponent = GetComponent<CollektiveNodeComponent>();
        }

        protected void ConnectTo(Node other) => _nodeComponent.Node.ConnectTo(other.Id);

        protected void SubscribeTo(Node other) => _nodeComponent.Node.SubscribeTo(other.Id);

        protected void DisconnectFrom(Node other) => _nodeComponent.Node.DisconnectFrom(other.Id);

        protected void UsubscribeFrom(Node other) => _nodeComponent.Node.UnsubscribeFrom(other.Id);

        protected void ConnectTo(int otherId) => _nodeComponent.Node.ConnectTo(otherId);

        protected void SubscribeTo(int otherId) => _nodeComponent.Node.SubscribeTo(otherId);

        protected void DisconnectFrom(int otherId) => _nodeComponent.Node.DisconnectFrom(otherId);

        protected void UsubscribeFrom(int otherId) => _nodeComponent.Node.UnsubscribeFrom(otherId);
    }
}
