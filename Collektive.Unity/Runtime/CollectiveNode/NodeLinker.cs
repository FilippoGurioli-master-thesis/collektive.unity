using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(CollektiveNodeComponent))]
    public abstract class NodeLinker : MonoBehaviour
    {
        private CollektiveNodeComponent _nodeComponent;

        private void Awake()
        {
            _nodeComponent = GetComponent<CollektiveNodeComponent>();
        }

        protected void ConnectTo(Node other)
        {
            //TODO
        }

        protected void SubscribeTo(Node other)
        {
            //TODO
        }

        protected void DisconnectFrom(Node other)
        {
            //TODO
        }

        protected void UsubscribeFrom(Node other)
        {
            //TODO
        }
    }
}
