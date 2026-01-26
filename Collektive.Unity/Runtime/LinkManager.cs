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
        private Color bidirectionalLinkColor = Color.cyan;

        [SerializeField]
        private Color monodirectionalLinkColor = Color.red;

        private Dictionary<(Node from, Node to), LineRenderer> _connections = new();

        public void AddDirectedConnection(Node from, Node to)
        {
            var key = (from, to);
            if (_connections.ContainsKey(key))
                return;
            var color = monodirectionalLinkColor;
            if (_connections.TryGetValue((to, from), out var lr))
            {
                lr.startColor = bidirectionalLinkColor;
                lr.endColor = bidirectionalLinkColor;
                color = bidirectionalLinkColor;
            }
            var lineObj = new GameObject($"link {from}->{to}");
            lineObj.transform.SetParent(transform);
            var lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            UpdateConnectionPosition(lineRenderer, from, to);
            _connections[key] = lineRenderer;
            lineRenderer.enabled = showLinks;
        }

        public void RemoveAllConnectionsForNode(Node node)
        {
            var keysToRemove = new List<(Node from, Node to)>();
            foreach (var key in _connections.Keys)
            {
                if (key.from == node || key.to == node)
                    keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
                RemoveDirectedConnection(key.from, key.to);
        }

        public void RemoveDirectedConnection(Node from, Node to)
        {
            var key = (from, to);
            if (_connections.TryGetValue(key, out LineRenderer lineRenderer))
            {
                Destroy(lineRenderer.gameObject);
                _connections.Remove(key);
                if (_connections.TryGetValue((to, from), out var lr))
                {
                    lr.startColor = monodirectionalLinkColor;
                    lr.endColor = monodirectionalLinkColor;
                }
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
                        lineRenderer.startColor = bidirectionalLinkColor;
                        lineRenderer.endColor = bidirectionalLinkColor;
                        lineRenderer.startWidth = lineWidth;
                        lineRenderer.endWidth = lineWidth;
                        lineRenderer.enabled = showLinks;
                    }
                }
            }
        }
    }
}
