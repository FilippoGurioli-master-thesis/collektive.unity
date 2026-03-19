using System.Collections.Generic;
using AYellowPaper;
using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(CollektiveNodeComponent))]
    public class Node : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        [Tooltip(
            "Clears current sensors and actuators lists and adds to them any sensor and actuator found in current game object and children"
        )]
        private bool automaticComponentsDetection;

        [Header("Components")]
        [SerializeField]
        private List<InterfaceReference<ISensor, MonoBehaviour>> sensors = new();

        [SerializeField]
        private List<InterfaceReference<IActuator, MonoBehaviour>> actuators = new();

        private List<ISensor> _sensors = new();
        private List<IActuator> _actuators = new();
        private CollektiveNodeComponent _nodeComponent;

        private void Awake()
        {
            _nodeComponent = GetComponent<CollektiveNodeComponent>();
            if (automaticComponentsDetection)
            {
                sensors.Clear();
                actuators.Clear();
                foreach (var actuator in GetComponentsInChildren<IActuator>())
                    actuators.Add(actuator);
                foreach (var sensor in GetComponentsInChildren<ISensor>())
                    sensors.Add(sensor);
            }
            _sensors = sensors.Select(s => s.Value).ToList();
            _actuators = actuators.Select(a => a.Value).ToList();
        }

        public SensorData Sense()
        {
            var data = new SensorData();
            foreach (var sensor in _sensors)
                data.MergeFrom(sensor.Sense());
            return data;
        }

        public void Compute()
        {
            //TODO
        }

        public void Act(ActuatorData data)
        {
            foreach (var actuator in _actuators)
                actuator.Act(data);
        }
    }
}
