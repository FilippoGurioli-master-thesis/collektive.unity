using System.Collections.Generic;
using AYellowPaper;
using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(CollektiveNodeComponent))]
    public class Node : MonoBehaviour
    {
        [
            SerializeField,
            Tooltip(
                "Clears current sensors and actuators lists and adds to them any sensor and actuator found in current node and children"
            )
        ]
        private bool detectSensorsAndActuators;

        [SerializeField]
        private List<InterfaceReference<ISensor, MonoBehaviour>> sensors = new();

        [SerializeField]
        private List<InterfaceReference<IActuator, MonoBehaviour>> actuators = new();

        private CollektiveNodeComponent _nodeComponent;

        private void Awake()
        {
            _nodeComponent = GetComponent<CollektiveNodeComponent>();
            if (detectSensorsAndActuators)
            {
                sensors.Clear();
                actuators.Clear();
                foreach (var actuator in GetComponentsInChildren<IActuator>())
                    actuators.Add(actuator);
                foreach (var sensor in GetComponentsInChildren<ISensor>())
                    sensors.Add(sensor);
            }
        }
    }
}
