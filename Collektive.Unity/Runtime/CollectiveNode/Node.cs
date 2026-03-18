using System.Collections.Generic;
using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    public class Node : MonoBehaviour
    {
        private readonly List<ISensor> _sensors = new();
        private readonly List<IActuator> _actuators = new();

        private void Start() { }
    }
}
