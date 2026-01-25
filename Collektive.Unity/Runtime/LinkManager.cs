using System.Collections.Generic;
using UnityEngine;

namespace Collektive.Unity
{
    public class LinkManager : SingletonBehaviour<LinkManager>
    {
        [SerializeField]
        private bool showLinks = true;

        [SerializeField]
        private float lineWidth = 0.05f;

        [SerializeField]
        private Color linkColor = Color.cyan;

        private Dictionary<(int, int), LineRenderer> _connections = new();

        public void AddConnection(int node1, int node2)
        {
            var key = GetOrderedPair(node1, node2);
            if (_connections.ContainsKey(key))
                return;
            var lineObj = new GameObject($"link {node1}-{node2}");
            lineObj.transform.SetParent(transform);
            var lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = linkColor;
            lineRenderer.endColor = linkColor;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            UpdateConnectionPosition(lineRenderer, node1, node2);
            _connections[key] = lineRenderer;
            lineRenderer.enabled = showLinks;
        }

        public void RemoveConnection(int node1, int node2)
        {
            var key = GetOrderedPair(node1, node2);
            if (_connections.TryGetValue(key, out LineRenderer lineRenderer))
            {
                Destroy(lineRenderer.gameObject);
                _connections.Remove(key);
            }
        }

        public void ClearAllConnections()
        {
            foreach (var lineRenderer in _connections.Values)
            {
                if (lineRenderer != null)
                    Destroy(lineRenderer.gameObject);
            }
            _connections.Clear();
        }

        private void Update()
        {
            foreach (var kvp in _connections)
            {
                var (node1, node2) = kvp.Key;
                UpdateConnectionPosition(kvp.Value, node1, node2);
            }
        }

        private void UpdateConnectionPosition(LineRenderer lineRenderer, int node1Id, int node2Id)
        {
            var nodes = SimulationManager.Instance.Nodes;
            if (nodes == null || lineRenderer == null)
                return;
            var n1 = nodes.Find(n => n.Id == node1Id);
            var n2 = nodes.Find(n => n.Id == node2Id);
            if (n1 != null && n2 != null)
            {
                lineRenderer.SetPosition(0, n1.transform.position);
                lineRenderer.SetPosition(1, n2.transform.position);
            }
        }

        private (int, int) GetOrderedPair(int node1, int node2) =>
            node1 < node2 ? (node1, node2) : (node2, node1);

        public void SetShowLinks(bool show)
        {
            showLinks = show;
            foreach (var lineRenderer in _connections.Values)
            {
                if (lineRenderer != null)
                    lineRenderer.enabled = show;
            }
        }

        private void OnValidate()
        {
            if (_connections != null)
            {
                foreach (var lineRenderer in _connections.Values)
                {
                    if (lineRenderer != null)
                    {
                        lineRenderer.startColor = linkColor;
                        lineRenderer.endColor = linkColor;
                        lineRenderer.startWidth = lineWidth;
                        lineRenderer.endWidth = lineWidth;
                        lineRenderer.enabled = showLinks;
                    }
                }
            }
        }
    }
}
