using System.Collections.Generic;
using System.Linq;
using Collektive.Unity.Schema;
using TNRD;
using UnityEngine;

namespace Collektive.Unity.CollectiveNode
{
    [RequireComponent(typeof(CollektiveNodeComponent), typeof(NodeScheduler))]
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
        private List<SerializableInterface<ISensor>> sensors = new();

        [SerializeField]
        private List<SerializableInterface<IActuator>> actuators = new();

        private List<ISensor> _sensors = new();
        private List<IActuator> _actuators = new();
        private CollektiveNodeComponent _nodeComponent;
        private INodeScheduler _scheduler;

        private void Awake()
        {
            _nodeComponent = GetComponent<CollektiveNodeComponent>();
            _scheduler = GetComponent<INodeScheduler>();
            if (automaticComponentsDetection)
            {
                sensors.Clear();
                actuators.Clear();
                foreach (var actuator in GetComponentsInChildren<IActuator>())
                    actuators.Add(new SerializableInterface<IActuator>(actuator));
                foreach (var sensor in GetComponentsInChildren<ISensor>())
                    sensors.Add(new SerializableInterface<ISensor>(sensor));
            }
            _sensors = sensors.Select(s => s.Value).ToList();
            _actuators = actuators.Select(a => a.Value).ToList();
            _scheduler.Cycle += Cycle;
        }

        private void OnDestroy() => _scheduler.Cycle -= Cycle;

        private void Cycle() => Act(_nodeComponent.Node.Compute(Sense()));

        private SensorData Sense()
        {
            var data = new SensorData();
            foreach (var sensor in _sensors)
                sensor.Contribute(data);
            return data;
        }

        private void Act(ActuatorData data)
        {
            foreach (var actuator in _actuators)
                actuator.Act(data);
        }
    }
}
