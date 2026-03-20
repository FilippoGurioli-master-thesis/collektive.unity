using UnityEngine;

namespace Collektive.Unity.Globals
{
    public class GlobalData : ScriptableObject
    {
        [SerializeField]
        [Tooltip(
            "Master seed to have reproducible simulations. All random generators starts from this"
        )]
        private int masterSeed = 42;

        [SerializeField]
        [Tooltip(
            "Maximum update frequency that a node can compute at."
                + "This is the fastest computational velocity. Every node should compute at a pace that is"
                + " a multiplier of this value"
        )]
        [Min(1f)]
        private float maxUpdateFrequency = 60f;

        [SerializeField]
        [Tooltip("TODO")]
        [Range(0.01f, 4)]
        private float timeScale = 1f;

        public int MasterSeed => masterSeed;

        public float MaxUpdateFrequency => maxUpdateFrequency;

        public float MinUpdatePeriod => 1f / MaxUpdateFrequency;
    }
}
