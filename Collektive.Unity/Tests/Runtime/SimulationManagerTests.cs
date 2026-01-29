using System.Collections;
using Collektive.Unity.Data;
using Collektive.Unity.Native;
using Collektive.Unity.Schema;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Collektive.Unity.Tests
{
    public class SimulationManagerTests
    {
        private class MockEngine : IEngine
        {
            public bool InitializeCalled;
            public int NodesAdded;
            public int NodesRemoved;
            public int Subscriptions;
            public int Unsubscriptions;
            public CustomGlobalData LastGlobalData;

            public void Initialize(GlobalData data) => InitializeCalled = true;

            public bool AddNode(int id)
            {
                NodesAdded++;
                return true;
            }

            public bool RemoveNode(int id)
            {
                NodesRemoved++;
                return true;
            }

            public NodeState Step(int id, SensorData data) => new NodeState();

            public bool Subscribe(int id1, int id2)
            {
                Subscriptions++;
                return true;
            }

            public bool Unsubscribe(int id1, int id2)
            {
                Unsubscriptions++;
                return true;
            }

            public void UpdateGlobalData(CustomGlobalData data) => LastGlobalData = data;
        }

        private GameObject _holder;
        private SimulationManager _manager;
        private MockEngine _mockEngine;

        [SetUp]
        public void Setup()
        {
            SimulationManager.ClearInstance();
            var existing = Object.FindObjectsByType<SimulationManager>(FindObjectsSortMode.None);
            foreach (var m in existing)
                Object.DestroyImmediate(m.gameObject);
            _holder = new GameObject("SimManagerTest");
            _holder.AddComponent<LinkManager>();
            _manager = _holder.AddComponent<SimulationManager>();
            _mockEngine = new MockEngine();
            _manager.Initialize(_mockEngine);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_holder);
            SimulationManager.ClearInstance();
        }

        [Test]
        public void AddNodeIncrementsIdAndCallsEngine()
        {
            var nodeGo = new GameObject();
            var node = nodeGo.AddComponent<NodeTests.TestNode>(); // Node.OnEnable already calls simuationManager.AddNode
            Assert.AreEqual(0, node.Id);
            Assert.AreEqual(1, _mockEngine.NodesAdded);
            Object.DestroyImmediate(nodeGo);
        }

        [Test]
        public void SubscribeCallsEngineAndLinkManager()
        {
            var n1 = new GameObject().AddComponent<NodeTests.TestNode>();
            var n2 = new GameObject().AddComponent<NodeTests.TestNode>();
            _manager.Subscribe(n1, n2);
            Assert.IsTrue(_mockEngine.Subscriptions == 1);
        }

        [Test]
        public void RemoveNodeCallsEngineAndRemovesFromDictionary()
        {
            var nodeGo = new GameObject();
            var node = nodeGo.AddComponent<NodeTests.TestNode>();
            _manager.AddNode(node);
            var removed = _manager.RemoveNode(node);
            Assert.IsTrue(removed);
            Assert.AreEqual(1, _mockEngine.NodesRemoved);
            Object.DestroyImmediate(nodeGo);
        }

        [Test]
        public void AddConnection_CallsSubscribeTwice()
        {
            var n1 = new GameObject().AddComponent<NodeTests.TestNode>();
            var n2 = new GameObject().AddComponent<NodeTests.TestNode>();
            _manager.AddConnection(n1, n2);
            Assert.AreEqual(2, _mockEngine.Subscriptions);
        }

        [Test]
        public void RemoveConnection_CallsUnsubscribeTwice()
        {
            var n1 = new GameObject().AddComponent<NodeTests.TestNode>();
            var n2 = new GameObject().AddComponent<NodeTests.TestNode>();
            _manager.RemoveConnection(n1, n2);
            Assert.AreEqual(2, _mockEngine.Unsubscriptions);
        }

        [Test]
        public void UpdateGlobalData_PassesDataToEngine()
        {
            var data = new CustomGlobalData();
            _manager.UpdateGlobalData(data);
            Assert.AreEqual(data, _mockEngine.LastGlobalData);
        }

        [UnityTest]
        public IEnumerator Update_StepsEngine_WhenNotPaused()
        {
            var nodeGo = new GameObject();
            var node = nodeGo.AddComponent<NodeTests.TestNode>();
            _manager.AddNode(node);
            var actCalled = false;
            node.OnStateReceived += (state) => actCalled = true;
            yield return null;
            Assert.IsTrue(actCalled, "Engine.Step should be called and result invoked on Node.");
            Object.DestroyImmediate(nodeGo);
        }

        [Test]
        public void Awake_ConfiguresUnityPhysicsAndTime()
        {
            Assert.AreEqual(SimulationMode.Script, Physics.simulationMode);
            Assert.AreEqual(0f, Time.timeScale);
        }
    }
}
