using System;

namespace Collektive.Unity.Abstractions
{
    public interface IScheduler
    {
        public event Action Cycle;
    }
}
