using System;
using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    public abstract class NodeScheduler : MonoBehaviour, INodeScheduler
    {
        public event Action Cycle;

        protected void Trigger() => Cycle?.Invoke();
    }
}
