using UnityEngine;

namespace Collektive.Unity.Examples.Scheduling
{
    public class FixedRateScheduler : AgentScheduler
    {
        [SerializeField]
        private int interval = 1;

        [SerializeField]
        private int offset = 0;

        private int _counter = 0;

        private void FixedUpdate()
        {
            _counter++;
            if (_counter > offset && (_counter - offset) % interval == 0)
                Trigger();
        }
    }
}
