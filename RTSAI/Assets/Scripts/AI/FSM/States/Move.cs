using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Move", menuName = "FSM/States/Move")]
public class Move : IStateBehaviour
{
    NavMeshAgent agent = null;

    public override void OnEnter(ref BlackBoard _blackBoard, GameObject _owner)
    {
        base.OnEnter(ref _blackBoard, _owner);

        if (!_owner.TryGetComponent<NavMeshAgent>(out agent))
        {
            Debug.LogError("Error: Failed to get NavMeshAgent!");
            return;
        }

        agent.destination = new Vector3(65f, 2f, 28f);
    }

    public override void OnUpdate(ref BlackBoard _blackBoard, GameObject _owner)
    {
        base.OnUpdate(ref _blackBoard, _owner);
    }

    public override void OnExit(ref BlackBoard _blackBoard, GameObject _owner)
    {
        base.OnExit(ref _blackBoard, _owner);
        agent.isStopped = true;
    }
}