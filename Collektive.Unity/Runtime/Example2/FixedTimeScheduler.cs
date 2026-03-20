using Collektive.Unity.CollectiveNode;
using Collektive.Unity.Globals;
using UnityEngine;

namespace Collektive.Unity.Example
{
    public class FixedRateScheduler : NodeScheduler
    {
        [SerializeField]
        private GlobalData data;

        [SerializeField]
        private int interval = 1;

        [SerializeField]
        private int offset = 0;

        private float _accumulator;
        private float Period => data.MinUpdatePeriod * interval;

        private void Start()
        {
            _accumulator = -data.MinUpdatePeriod * offset;
        }

        private void FixedUpdate()
        {
            _accumulator += Time.fixedDeltaTime;
            if (_accumulator >= Period)
            {
                _accumulator -= Period;
                Trigger();
            }
        }
    }
}
