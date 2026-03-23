using Collektive.Unity.Schema;

namespace Collektive.Unity.Abstractions
{
    /// <summary>
    /// Interface representing a sensor.
    /// </summary>
    public interface ISensor
    {
        /// <summary>
        /// Sense the environment and add its sensed data to the data class.
        /// </summary>
        void Contribute(SensorData data);
    }
}
