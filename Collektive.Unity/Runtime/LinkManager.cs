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

        private Dictionary<(Node, Node), LineRenderer> _connections = new();

        public void AddConnection(Node node1, Node node2)
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

        public void RemoveAllConnectionsForNode(Node node)
        {
            var keysToRemove = new List<(Node, Node)>();
            foreach (var key in _connections.Keys)
            {
                if (key.Item1 == node || key.Item2 == node)
                    keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
                RemoveConnection(key.Item1, key.Item2);
        }

        public void RemoveConnection(Node node1, Node node2)
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

        private void UpdateConnectionPosition(LineRenderer lineRenderer, Node node1, Node node2)
        {
            lineRenderer.SetPosition(0, node1.transform.position);
            lineRenderer.SetPosition(1, node2.transform.position);
        }

        private (Node, Node) GetOrderedPair(Node node1, Node node2) =>
            node1.Id < node2.Id ? (node1, node2) : (node2, node1);

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
