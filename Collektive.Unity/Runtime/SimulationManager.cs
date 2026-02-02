using System.Collections.Generic;
using System.Linq;
using Collektive.Unity.Attributes;
using Collektive.Unity.Data;
using Collektive.Unity.Native;
using Collektive.Unity.Schema;
using Collektive.Unity.Utils;
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

        [SerializeField, ReadOnly, Tooltip("Accumulated timestep")]
        private float accumulator;

        [
            SerializeField,
            Tooltip("Period that passes between one cycle and the next one"),
            Range(0.001f, MAX_TIMESTEP)
        ]
        private float deltaTime = 0.02f;

        [SerializeField, Tooltip("Set to true to pause global simulation")]
        private bool globalSimulationPaused = false;

        [SerializeField, Tooltip("The master seed from which every random generator begins")]
        private int masterSeed = 42;

        private Dictionary<int, Node> _nodes = new();
        private LinkManager _linkManager;
        private bool _isEngineInit = false;
        private int _counter = 0;
        private IEngine _engine;
        private const float MAX_TIMESTEP = 0.3f;

        public GlobalData GlobalData { get; private set; }

        public void Initialize(IEngine engine)
        {
            _engine = engine;
        }

        private void Awake()
        {
            _linkManager = GetComponent<LinkManager>();
            _engine ??= new EngineNativeApi();
            InitIfNotPresent();
            Physics.simulationMode = SimulationMode.Script;
        }

        private void InitIfNotPresent()
        {
            if (_isEngineInit)
                return;
            _isEngineInit = true;
            GlobalData = new GlobalData { Seed = masterSeed };
            _engine.Initialize(GlobalData);
        }

        private void Update()
        {
            if (globalSimulationPaused)
                return;
            accumulator = Mathf.Min(accumulator + Time.unscaledDeltaTime, MAX_TIMESTEP + 0.1f);
            while (deltaTime <= accumulator)
            {
                SimulationStep();
                accumulator -= deltaTime;
            }
        }

        private void SimulationStep()
        {
            foreach (var (id, node) in _nodes)
            {
                var sensing = node.Sense();
                var state = _engine.Step(id, sensing);
                node.OnStateReceived?.Invoke(state);
            }
            Physics.Simulate(deltaTime);
        }

        public bool AddConnection(Node node1, Node node2)
        {
            var sub1 = Subscribe(node1, node2);
            var sub2 = Subscribe(node2, node1);
            return sub1 && sub2;
        }

        public bool RemoveConnection(Node node1, Node node2)
        {
            var unsub1 = Unsubscribe(node1, node2);
            var unsub2 = Unsubscribe(node2, node1);
            return unsub1 && unsub2;
        }

        public bool Subscribe(Node node1, Node node2)
        {
            _linkManager.AddDirectedConnection(node1, node2);
            return _engine.Subscribe(node1.Id, node2.Id);
        }

        public bool Unsubscribe(Node node1, Node node2)
        {
            _linkManager.RemoveDirectedConnection(node1, node2);
            return _engine.Unsubscribe(node1.Id, node2.Id);
        }

        public void UpdateGlobalData(CustomGlobalData data) => _engine.UpdateGlobalData(data);

        public int AddNode(Node node)
        {
            InitIfNotPresent();
            var id = GetNewId();
            if (_engine.AddNode(id))
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
            if (_nodes.Values.Contains(node) && _engine.RemoveNode(node.Id))
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
