using Collektive.Unity.Generated;
using UnityEngine;

namespace Collektive.Unity
{
    /// <summary>
    /// This is a test documentation.
    /// </summary>
    public class TestScript : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(NativeBackend.ProcessDinosaur(new Dinosaur { Name = "Test Dino" }));
        }
    }
}
