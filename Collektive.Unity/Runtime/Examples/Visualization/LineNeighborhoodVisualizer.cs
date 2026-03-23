using System.Collections.Generic;
using Collektive.Unity.CollectiveNode;
using Collektive.Unity.Globals;
using UnityEngine;

namespace Collektive.Unity.Examples.Visualization
{
    public class LineNeighborhoodVisualizer : NeighborhoodVisualizer
    {
        [SerializeField]
        private SimulationSettings data;

        private Dictionary<Agent, LineRenderer> _connections = new();

        protected override void OnSubscribe(Agent node)
        {
            if (_connections.ContainsKey(node))
                return;
            var lineObj = new GameObject($"link {name}->{node.name}");
            lineObj.transform.SetParent(transform);
            var lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.sharedMaterial = data.LinkMaterial;
            lineRenderer.startColor = data.LinkColor.mono;
            lineRenderer.endColor = data.LinkColor.mono;
            lineRenderer.startWidth = data.LineWidth;
            lineRenderer.endWidth = data.LineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            UpdateConnectionPosition(lineRenderer, node);
            lineRenderer.enabled = data.ShowNeighborhood;
            _connections.Add(node, lineRenderer);
        }

        protected override void OnUnsubscribe(Agent node)
        {
            if (_connections.TryGetValue(node, out LineRenderer lineRenderer))
            {
                Destroy(lineRenderer.gameObject);
                _connections.Remove(node);
            }
        }

        private void UpdateConnectionPosition(LineRenderer lineRenderer, Agent target)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
        }

        private void Update()
        {
            foreach (var (target, lr) in _connections)
            {
                lr.enabled = data.ShowNeighborhood;
                UpdateConnectionPosition(lr, target);
            }
        }
    }
}
