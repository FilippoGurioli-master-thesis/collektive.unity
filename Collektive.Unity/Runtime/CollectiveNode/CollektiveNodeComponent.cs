namespace Collektive.Unity.CollectiveNode
{
    [DisallowMultipleComponent]
    public class CollektiveNodeComponent : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private int id;

        public CollektiveNode Node { get; private set; }

        private void OnEnable()
        {
            Node = new CollektiveNode(GetInstanceID());
            id = Node.Id;
            name = $"node {id}";
        }

        private void OnDisable() => Node?.Dispose();
    }
}
