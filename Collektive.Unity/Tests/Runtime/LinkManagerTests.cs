using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Collektive.Unity.Tests
{
    public class LinkManagerTests
    {
        private LinkManager _linkManager;
        private NodeTests.TestNode _nodeA;
        private NodeTests.TestNode _nodeB;

        [SetUp]
        public void Setup()
        {
            var existing = Object.FindObjectsByType<LinkManager>(FindObjectsSortMode.None);
            foreach (var m in existing)
                Object.DestroyImmediate(m.gameObject);
            var go = new GameObject("LinkManager");
            _linkManager = go.AddComponent<LinkManager>();
            _nodeA = new GameObject("NodeA").AddComponent<NodeTests.TestNode>();
            _nodeB = new GameObject("NodeB").AddComponent<NodeTests.TestNode>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_nodeA.gameObject);
            Object.DestroyImmediate(_nodeB.gameObject);
            Object.DestroyImmediate(_linkManager.gameObject);
        }

        [UnityTest]
        public IEnumerator AddDirectedConnectionCreatesGameObject()
        {
            _linkManager.AddDirectedConnection(_nodeA, _nodeB);
            var linkObj = GameObject.Find($"link {_nodeA}->{_nodeB}");
            Assert.IsNotNull(linkObj);
            Assert.IsNotNull(linkObj.GetComponent<LineRenderer>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator BidirectionalConnectionChangesColor()
        {
            _linkManager.AddDirectedConnection(_nodeA, _nodeB);
            var lrAB = GameObject.Find($"link {_nodeA}->{_nodeB}").GetComponent<LineRenderer>();
            var initialColor = lrAB.startColor;
            _linkManager.AddDirectedConnection(_nodeB, _nodeA);
            var lrBA = GameObject.Find($"link {_nodeB}->{_nodeA}").GetComponent<LineRenderer>();
            Assert.AreEqual(lrAB.startColor, lrBA.startColor);
            Assert.AreNotEqual(
                initialColor,
                lrAB.startColor,
                "Color should have changed to bidirectional cyan."
            );
            yield return null;
        }

        [UnityTest]
        public IEnumerator RemoveAllConnectionsForNodeCleansUpRenderers()
        {
            _linkManager.AddDirectedConnection(_nodeA, _nodeB);
            _linkManager.AddDirectedConnection(_nodeB, _nodeA);
            _linkManager.RemoveAllConnectionsForNode(_nodeA);
            yield return null;
            Assert.IsNull(GameObject.Find($"link {_nodeA}->{_nodeB}"));
            Assert.IsNull(GameObject.Find($"link {_nodeB}->{_nodeA}"));
        }

        [UnityTest]
        public IEnumerator SetShowLinksTogglesLineRenderers()
        {
            _linkManager.AddDirectedConnection(_nodeA, _nodeB);
            var lr = GameObject.Find($"link {_nodeA}->{_nodeB}").GetComponent<LineRenderer>();
            _linkManager.SetShowLinks(false);
            Assert.IsFalse(lr.enabled);
            _linkManager.SetShowLinks(true);
            Assert.IsTrue(lr.enabled);
            yield return null;
        }
    }
}
