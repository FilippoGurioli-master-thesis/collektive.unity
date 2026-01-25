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

        [SerializeField, Tooltip("Set to true to pause global simulation")]
        private bool globalSimulationPaused = false;

        [SerializeField, Tooltip("The master seed from which every random generator begins")]
        private int masterSeed = 42;

        private List<Node> _nodes = new();
        private LinkManager _linkManager;

        public List<Node> Nodes => new(_nodes);
        public GlobalData GlobalData { get; private set; }

        private void Awake()
        {
            _linkManager = GetComponent<LinkManager>();
            GlobalData = new GlobalData { Seed = masterSeed };
            EngineNativeApi.Initialize(GlobalData);
            Physics.simulationMode = SimulationMode.Script;
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (globalSimulationPaused)
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

        public int AddNode(Node node)
        {
            var id = _nodes.Count;
            if (EngineNativeApi.AddNode(id))
            {
                _nodes.Add(node);
                return id;
            }
            else
            {
                Debug.LogError($"Native Engine return false on adding node {id}");
                return 0;
            }
        }
    }
}
