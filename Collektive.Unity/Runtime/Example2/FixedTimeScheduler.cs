namespace Collektive.Unity.Example
{
    public class FixedTimeScheduler : NodeScheduler
    {
        [SerializeField]
        private GlobalData data;

        [SerializeField]
        private int interval = 1;

        [SerializeField]
        private int offset = 0;

        private void FixedUpdate() { }
    }
}
