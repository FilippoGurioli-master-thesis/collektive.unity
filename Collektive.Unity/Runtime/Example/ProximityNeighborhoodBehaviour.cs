using UnityEngine;

namespace Collektive.Unity.Example
{
    [RequireComponent(typeof(Collider))]
    public class ProximityNeighborhoodBehaviour : MonoBehaviour
    {
        private Collider _collider;
        private Node _node;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _node = GetComponentInParent<Node>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherNode = other.GetComponent<Node>();
            if (otherNode != null)
            {
                SimulationManager.Instance.AddConnection(_node, otherNode);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var otherNode = other.GetComponent<Node>();
            if (otherNode != null)
            {
                SimulationManager.Instance.RemoveConnection(_node, otherNode);
            }
        }
    }
}
