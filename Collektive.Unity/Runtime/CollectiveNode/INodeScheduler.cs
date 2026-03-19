using System;

namespace Collektive.Unity.CollectiveNode
{
    public interface INodeScheduler
    {
        public event Action Cycle;
    }
}
