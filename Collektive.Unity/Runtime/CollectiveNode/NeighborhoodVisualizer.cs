using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(NeighboringComponent))]
    public abstract class NeighborhoodVisualizer : MonoBehaviour
    {
        private NeighboringComponent _logic;

        protected virtual void Awake()
        {
            _logic = GetComponent<NeighboringComponent>();
            _logic.OnSubscribe += OnSubscribe;
            _logic.OnUnsubscribe += OnUnsubscribe;
        }

        protected abstract void OnSubscribe(Node node);
        protected abstract void OnUnsubscribe(Node node);

        protected virtual void OnDestroy()
        {
            _logic.OnSubscribe -= OnSubscribe;
            _logic.OnUnsubscribe -= OnUnsubscribe;
        }
    }
}
