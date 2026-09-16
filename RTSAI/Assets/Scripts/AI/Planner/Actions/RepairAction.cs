using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RepairAction : GoapAction
{
    public RepairAction()
    {
        actionName = "Repair";
        cost = 1;
        SetPreconditions(WorldStateDefiner.IsAtLocation, WorldStateDefiner.None);
        SetEffects(WorldStateDefiner.IsTargetRepaired, WorldStateDefiner.None);
    }

    protected override void StartAction()
    {
        foreach (Unit unit in squad.Units)
        {
            unit.SetRepairTarget(squad.CurrentTarget.EntityTarget);
        }
    }

    protected override void UpdateAction()
    {
        foreach (Unit unit in squad.Units)
        {
            if (unit.IsTriggered)
            unit.ComputeRepairing();
        }

        if (!squad.CurrentTarget.EntityTarget.NeedsRepairing())
        {
            bIsDone = true;
        }
    }

    public override void PauseAction(Unit unit)
    {
        unit.Target.EntityTarget = null;
    }

    public override void UnpauseAction(Unit unit)
    {
        BaseEntity target = squad.CurrentTarget.EntityTarget;
        if (unit.CanRepair(target) && target.GetTeam() == unit.GetTeam())
        {
            unit.SetAttackTarget(target);
        }
    }
}
