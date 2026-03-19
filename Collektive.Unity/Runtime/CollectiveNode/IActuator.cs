using Collektive.Unity.Schema;

namespace Collektive.Unity.CollectiveNode
{
    public interface IActuator
    {
        void Act(ActuatorData data);
    }

    /// <summary>
    /// Interface representing an actuator.
    /// </summary>
    public interface IActuator<in T> : IActuator
        where T : IMessage<T>
    {
        /// <summary>
        /// Apply the changes taken as input to the current state and the surrounding environment.
        /// </summary>
        void Act(T data);

        void IActuator.Act(ActuatorData data) => Act(Extract(data));

        T Extract(ActuatorData data);
    }
}
