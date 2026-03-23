using UnityEngine;

namespace Collektive.Unity.Examples.Configuration
{
    [CreateAssetMenu(fileName = "SimulationSettings", menuName = "Collektive/Simulation Settings")]
    public class SimulationSettings : ScriptableObject
    {
        [Header("Simulation configurations")]
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
        [Tooltip("The time scale at which let the simulation run at.")]
        [Range(0.01f, 4)]
        private float timeScale = 1f;

        [Header("Neighborhood viz")]
        [SerializeField]
        private bool showNeighborhood = true;

        [Header("Actuators")]
        [SerializeField]
        [Min(0.01f)]
        private float forceMagnitude = 2f;

        [SerializeField]
        private float lineWidth = 0.05f;

        [SerializeField]
        private Color bidirectionalLinkColor = Color.cyan;

        [SerializeField]
        private Color monodirectionalLinkColor = Color.red;

        [SerializeField]
        private Material linkMaterial;

        public int MasterSeed => masterSeed;

        public float MaxUpdateFrequency => maxUpdateFrequency;

        public float MinUpdatePeriod => 1f / MaxUpdateFrequency;

        public float TimeScale => timeScale;

        public bool ShowNeighborhood => showNeighborhood;

        public (Color mono, Color bi) LinkColor =>
            (monodirectionalLinkColor, bidirectionalLinkColor);

        public float LineWidth => lineWidth;

        public Material LinkMaterial => linkMaterial;

        public float ForceMagnitude => forceMagnitude;

        private void OnEnable()
        {
            Time.fixedDeltaTime = MinUpdatePeriod;
        }
    }
}
