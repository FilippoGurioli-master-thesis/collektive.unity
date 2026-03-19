using Collektive.Unity.Schema;

namespace Collektive.Unity.CollectiveNode
{
    public interface ISensor
    {
        IMessage Sense();
    }

    /// <summary>
    /// Interface representing a sensor.
    /// </summary>
    public interface ISensor<out T> : ISensor
        where T : IMessage<T>
    {
        /// <summary>
        /// Sense the environment and return the data structure representing the value sensed.
        /// </summary>
        T Sense();

        IMessage ISensor.Sense() => Sense();
    }
}
