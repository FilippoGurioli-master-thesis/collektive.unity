using Collektive.Unity.Schema;

namespace Collektive.Unity.CollectiveNode
{
    /// <summary>
    /// Interface representing a sensor.
    /// </summary>
    public interface ISensor
    {
        /// <summary>
        /// Sense the environment and return the data structure representing the value sensed.
        /// </summary>
        SensorData Sense();
    }
}
