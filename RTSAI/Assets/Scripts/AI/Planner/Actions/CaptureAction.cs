using UnityEngine;

public class CaptureAction : GoapAction
{
    public CaptureAction()
    {
        actionName = "Capture";
        cost = 2;
        SetPreconditions(WorldStateDefiner.IsAtLocation, WorldStateDefiner.None);
        SetEffects(WorldStateDefiner.IsTargetCaptured, WorldStateDefiner.None);
    }

    protected override void StartAction()
    {
        foreach (Unit unit in squad.Units)
        {
            unit.StartCapture(squad.CurrentTarget.CaptureTarget);
        }
    }

    protected override void UpdateAction()
    {
        if (squad.CurrentTarget.CaptureTarget.GetTeam() == squad.GetTeam())
            bIsDone = true;
    }

    public override void PauseAction(Unit unit)
    {
        unit.StopCapture();
    }

    public override void UnpauseAction(Unit unit)
    {
        unit.StartCapture(squad.CurrentTarget.CaptureTarget);
    }
}
