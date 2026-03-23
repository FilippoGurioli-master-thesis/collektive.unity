using System;
using UnityEngine;

namespace Collektive.Unity.Core
{
    public abstract class AgentScheduler : MonoBehaviour, IScheduler
    {
        public event Action Cycle;

        protected void Trigger() => Cycle?.Invoke();
    }
}
