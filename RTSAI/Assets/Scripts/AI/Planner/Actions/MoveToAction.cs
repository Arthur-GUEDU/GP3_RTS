using UnityEngine;

public class MoveToAction : GoapAction
{
    private float stoppingDistance = 0.1f;

    public MoveToAction()
    {
        actionName = "MoveTo";
        cost = 1;
        SetPreconditions(WorldStateDefiner.None, WorldStateDefiner.IsAtLocation);
        SetEffects(WorldStateDefiner.IsAtLocation, WorldStateDefiner.None);
    }

    protected override void StartAction()
    {
        squad.SetTargetPos(squad.CurrentTarget.PositionTarget);
        foreach (Unit unit in squad.Units)
        {
            unit.NavMeshAgent.isStopped = false;
        }
    }

    protected override void UpdateAction()
    {
        squad.UpdateUnitsTargetPos();

        if (squad.NavMeshAgent.remainingDistance <= stoppingDistance)
            bIsDone = true;
        
    }

    protected override void EndAction()
    {
        squad.NavMeshAgent.isStopped = true;
        foreach (Unit unit in squad.Units)
        {
            unit.NavMeshAgent.isStopped = true;
        }
    }

    public override void PauseAction(Unit unit)
    {
        unit.NavMeshAgent.isStopped = true;
    }

    public override void UnpauseAction(Unit unit)
    {
        unit.NavMeshAgent.isStopped = false;
    }
}
