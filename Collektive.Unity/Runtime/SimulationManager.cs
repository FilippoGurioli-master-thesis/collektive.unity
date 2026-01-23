using System.Collections.Generic;
using Collektive.Unity.Attributes;
using Collektive.Unity.Data;
using Collektive.Unity.Native;
using Collektive.Unity.Schema;
using UnityEngine;

namespace Collektive.Unity
{
    /// <summary>
    /// Handles the logic part of the simulation.
    /// </summary>
    [RequireComponent(typeof(LinkManager))]
    public class SimulationManager : SingletonBehaviour<SimulationManager>
    {
        [SerializeField, ReadOnly, Tooltip("The total number of nodes in the simulation")]
        private int totalNodes;

        [SerializeField, Tooltip("Period that passes between one cycle and the next one")]
        private float deltaTime = 0.02f;

        [SerializeField, Tooltip("Set to true to pause simulation")]
        private bool simulationPaused = false;

        private List<Node> _nodes = new();
        private LinkManager _linkManager;

        public GlobalData GlobalData { get; private set; }

        private void Awake()
        {
            var nodes = Object.FindObjectsByType<Node>(FindObjectsSortMode.None);
            _linkManager = GetComponent<LinkManager>();
            _nodes.AddRange(nodes);
            _linkManager.SetNodes(_nodes);
            totalNodes = _nodes.Count;
            GlobalData = new GlobalData { TotalNodes = nodes.Length, DeltaTime = deltaTime };
            EngineNativeApi.Initialize(GlobalData);
            for (var i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Id = i;
                _nodes[i].name = $"node {i}";
            }
            Physics.simulationMode = SimulationMode.Script;
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (simulationPaused)
                return;
            foreach (var node in _nodes)
            {
                var sensing = node.Sense();
                var state = EngineNativeApi.Step(_nodes.IndexOf(node), sensing);
                node.OnStateReceived?.Invoke(state);
            }
            Physics.Simulate(deltaTime);
        }

        public bool AddConnection(int node1, int node2)
        {
            _linkManager.AddConnection(node1, node2);
            return EngineNativeApi.AddConnection(node1, node2);
        }

        public bool RemoveConnection(int node1, int node2)
        {
            _linkManager.RemoveConnection(node1, node2);
            return EngineNativeApi.RemoveConnection(node1, node2);
        }

        public void UpdateGlobalData(CustomGlobalData data) =>
            EngineNativeApi.UpdateGlobalData(data);
    }
}
