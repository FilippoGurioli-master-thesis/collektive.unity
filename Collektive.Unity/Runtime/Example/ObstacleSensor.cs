using System.Collections.Generic;
using UnityEngine;

namespace Collektive.Unity.Example
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObstacleSensor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float detectionRadius = 5f;

        [SerializeField]
        private LayerMask obstacleLayer;

        /// <summary>
        /// Returns a list of vectors that starts from the node and ends to the obstacle
        /// </summary>
        public List<Vector3> Sense()
        {
            var obstacleVectors = new List<Vector3>();
            var hits = Physics.OverlapSphere(transform.position, detectionRadius, obstacleLayer);
            foreach (var hit in hits)
            {
                var closestPoint = hit.ClosestPoint(transform.position);
                var relativeVector = closestPoint - transform.position;
                if (relativeVector == Vector3.zero)
                    relativeVector = hit.transform.position - transform.position;
                obstacleVectors.Add(relativeVector);
            }
            return obstacleVectors;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            if (Application.isPlaying)
            {
                var vectors = Sense();
                foreach (var v in vectors)
                    Gizmos.DrawRay(transform.position, v);
            }
        }
    }
}
