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
    public class SimulationManager : SingletonBehaviour<SimulationManager>
    {
        [SerializeField, ReadOnly, Tooltip("The total number of nodes in the simulation")]
        private int totalNodes;

        [SerializeField, Tooltip("Period that passes between one cycle and the next one")]
        private float deltaTime = 0.02f;

        private List<Node> _nodes = new();

        private void Awake()
        {
            var nodes = Object.FindObjectsByType<Node>(FindObjectsSortMode.None);
            _nodes.AddRange(nodes);
            totalNodes = _nodes.Count;
            EngineNativeApi.Initialize(
                new GlobalData { TotalNodes = nodes.Length, DeltaTime = deltaTime }
            );
            Physics.simulationMode = SimulationMode.Script;
        }

        private void FixedUpdate()
        {
            var sensing = new List<SensorData>();
            foreach (var node in _nodes)
                sensing.Add(node.Sense());
            var states = EngineNativeApi.Step(sensing);
            for (var i = 0; i < _nodes.Count; i++)
                _nodes[i].OnStateReceived?.Invoke(states[i]);
        }

        public bool AddConnection(int node1, int node2) =>
            EngineNativeApi.AddConnection(node1, node2);

        public bool RemoveConnection(int node1, int node2) =>
            EngineNativeApi.RemoveConnection(node1, node2);

        public void UpdateGlobalData(CustomGlobalData data) =>
            EngineNativeApi.UpdateGlobalData(data);
    }
}
