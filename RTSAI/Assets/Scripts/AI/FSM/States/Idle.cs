using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "FSM/States/Idle")]
public class Idle : IStateBehaviour
{
    public override void OnEnter(ref BlackBoard _blackBoard, GameObject _owner)
    {
        base.OnEnter(ref _blackBoard, _owner);
    }

    public override void OnUpdate(ref BlackBoard _blackBoard, GameObject _owner)
    {
        base.OnUpdate(ref _blackBoard, _owner);
    }

    public override void OnExit(ref BlackBoard _blackBoard, GameObject _owner)
    {
        base.OnExit(ref _blackBoard, _owner);
    }
}