using UnityEngine;

namespace Collektive.Unity.Core.Neighborhood
{
    [RequireComponent(typeof(NeighborhoodComponent))]
    public abstract class NeighborhoodVisualizer : MonoBehaviour
    {
        private NeighborhoodComponent _logic;

        protected virtual void Awake()
        {
            _logic = GetComponent<NeighborhoodComponent>();
            _logic.OnSubscribe += OnSubscribe;
            _logic.OnUnsubscribe += OnUnsubscribe;
        }

        protected abstract void OnSubscribe(Agent node);
        protected abstract void OnUnsubscribe(Agent node);

        protected virtual void OnDestroy()
        {
            _logic.OnSubscribe -= OnSubscribe;
            _logic.OnUnsubscribe -= OnUnsubscribe;
        }
    }
}
