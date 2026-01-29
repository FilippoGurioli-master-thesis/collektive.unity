using System.Collections;
using Collektive.Unity;
using Collektive.Unity.Schema;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Collektive.Unity.Tests
{
    public class NodeTests
    {
        public class TestNode : Node
        {
            public NodeState LastReceivedState;

            public override SensorData Sense() => new SensorData();

            protected override void Act(NodeState state) => LastReceivedState = state;

            public float PublicGetNextRandom() => GetNextRandom();

            public float PublicGetGaussian(float mean, float stdDev) => GetGaussian(mean, stdDev);
        }

        [UnityTest]
        public IEnumerator RegistersWithManager_OnEnable()
        {
            var managerGo = new GameObject("SimulationManager");
            managerGo.AddComponent<SimulationManager>();
            var nodeGo = new GameObject("TestNode");
            var node = nodeGo.AddComponent<TestNode>();
            yield return null;
            Assert.AreNotEqual(0, node.Id, "Node ID should be assigned by SimulationManager.");
            Assert.AreEqual($"node {node.Id}", node.name, "GameObject name should match ID.");
            Object.DestroyImmediate(nodeGo);
            Object.DestroyImmediate(managerGo);
        }

        [Test]
        public void MathMethods_ReturnValues()
        {
            var go = new GameObject();
            var node = go.AddComponent<TestNode>();
            var rand = node.PublicGetNextRandom();
            var gaussian = node.PublicGetGaussian(0f, 1f);
            Assert.IsTrue(rand >= 0f && rand <= 1f);
            Assert.IsFalse(float.IsNaN(gaussian));
            Object.DestroyImmediate(go);
        }

        [Test]
        public void OnStateReceived_TriggersAct()
        {
            var go = new GameObject();
            var node = go.AddComponent<TestNode>();
            var testState = new NodeState();
            node.OnStateReceived?.Invoke(testState);
            Assert.AreEqual(
                testState,
                node.LastReceivedState,
                "Act should have been called via the Action."
            );
            Object.DestroyImmediate(go);
        }
    }
}
