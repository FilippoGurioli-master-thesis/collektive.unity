using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<int, Node> _nodes = new();
        private LinkManager _linkManager;
        private bool _isEngineInit = false;
        private int _counter = 0;

        public List<Node> Nodes => new(_nodes.Values);
        public GlobalData GlobalData { get; private set; }

        private void Awake()
        {
            _linkManager = GetComponent<LinkManager>();
            InitIfNotPresent();
            Physics.simulationMode = SimulationMode.Script;
            Time.timeScale = 0f;
        }

        private void InitIfNotPresent()
        {
            if (_isEngineInit)
                return;
            _isEngineInit = true;
            GlobalData = new GlobalData { Seed = masterSeed };
            EngineNativeApi.Initialize(GlobalData);
        }

        private void Update()
        {
            if (globalSimulationPaused)
                return;
            foreach (var (id, node) in _nodes)
            {
                var sensing = node.Sense();
                var state = EngineNativeApi.Step(id, sensing);
                node.OnStateReceived?.Invoke(state);
            }
            Physics.Simulate(deltaTime);
        }

        public bool AddConnection(Node node1, Node node2)
        {
            _linkManager.AddConnection(node1, node2);
            return EngineNativeApi.AddConnection(node1.Id, node2.Id);
        }

        public bool RemoveConnection(Node node1, Node node2)
        {
            _linkManager.RemoveConnection(node1, node2);
            return EngineNativeApi.RemoveConnection(node1.Id, node2.Id);
        }

        public void UpdateGlobalData(CustomGlobalData data) =>
            EngineNativeApi.UpdateGlobalData(data);

        public int AddNode(Node node)
        {
            InitIfNotPresent();
            var id = GetNewId();
            if (EngineNativeApi.AddNode(id))
            {
                _nodes.Add(id, node);
                return id;
            }
            else
            {
                Debug.LogError($"Native Engine return false on adding node {id}");
                return 0;
            }
        }

        public bool RemoveNode(Node node)
        {
            InitIfNotPresent();
            if (_nodes.Values.Contains(node) && EngineNativeApi.RemoveNode(node.Id))
            {
                _linkManager.RemoveAllConnectionsForNode(node);
                return _nodes.Remove(node.Id);
            }
            else
            {
                Debug.LogError($"Native Engine return false on removing node {node.Id}");
                return false;
            }
        }

        private int GetNewId() => _counter++;
    }
}
