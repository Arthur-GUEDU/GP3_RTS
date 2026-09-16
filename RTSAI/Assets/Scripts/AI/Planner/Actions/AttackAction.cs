using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackAction : GoapAction
{
    BaseEntity m_target;

    public AttackAction()
    {
        actionName = "Attack";
        cost = 1;
        SetPreconditions(WorldStateDefiner.IsAtLocation, WorldStateDefiner.None);
        SetEffects(WorldStateDefiner.IsTargetDestroyed, WorldStateDefiner.None);
    }

    protected override void StartAction()
    {
        BaseEntity target = squad.CurrentTarget.EntityTarget;
        m_target = target;
        foreach (Unit unit in squad.Units)
        {
            if (unit.CanAttack(target) && target.GetTeam() != unit.GetTeam())
            {
                unit.SetAttackTarget(target);
            }
        }
    }

    protected override void UpdateAction()
    {
        foreach (Unit unit in squad.Units)
        {
            unit.ComputeAttack();
        }

        if (!m_target.IsAlive)
            bIsDone = true;
    }

    public override void PauseAction(Unit unit)
    {
        unit.Target.EntityTarget = null;
    }

    public override void UnpauseAction(Unit unit)
    {
        BaseEntity target = squad.CurrentTarget.EntityTarget;
        if (unit.CanAttack(target) && target.GetTeam() != unit.GetTeam())
        {
            unit.SetAttackTarget(target);
        }
    }
}
