using Collektive.Unity.Schema;

namespace Collektive.Unity.CollectiveNode
{
    /// <summary>
    /// Interface representing an actuator.
    /// </summary>
    public interface IActuator
    {
        /// <summary>
        /// Apply the changes taken as input to the current state and the surrounding environment.
        /// </summary>
        void Act(ActuatorData data);
    }
}
