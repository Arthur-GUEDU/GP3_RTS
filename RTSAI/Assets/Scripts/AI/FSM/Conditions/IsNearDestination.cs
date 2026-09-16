using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "IsNearDestination", menuName = "FSM/Conditions/IsNearDestination")]
public class IsNearDestination : ICondition
{
    public override bool IsValid(BlackBoard _blackBoard, GameObject _owner)
    {
        NavMeshAgent agent;
        if (_owner.TryGetComponent(out agent))
        {
            float distance = Vector3.Distance(agent.destination, _owner.transform.position);
            bool cond = distance <= 1;
            return bIsInverted ? !cond : cond;
        }
        return bIsInverted;
    }
}