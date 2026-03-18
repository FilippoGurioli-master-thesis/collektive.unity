using Collektive.Unity.Schema;

namespace Collektive.Unity.CollectiveNode
{
    public interface IActuator
    {
        void Act(ActuatorData data);
    }
}
