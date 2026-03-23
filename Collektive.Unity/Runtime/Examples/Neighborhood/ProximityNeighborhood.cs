using System.Linq;
using UnityEngine;

namespace Collektive.Unity.Examples.Neighborhood
{
    public class ProximityNeighborhood : NeighborhoodComponent
    {
        private void Start()
        {
            Debug.Assert(
                GetComponents<Collider>().Any(c => c.isTrigger),
                "ProximityNeighboring: proximityCollider's GameObject must have at least one trigger collider.",
                this
            );
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Agent>(out var node))
                SubscribeTo(node);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Agent>(out var node))
                UnsubscribeFrom(node);
        }
    }
}
