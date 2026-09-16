using UnityEngine;

public class DefendAction : GoapAction
{
    float radius = 8f;
    float defendTime = 5f;
    float stopTime;

    public DefendAction()
    {
        actionName = "Defend";
        cost = 2;
        SetPreconditions(WorldStateDefiner.IsAtLocation, WorldStateDefiner.None);
        SetEffects(WorldStateDefiner.IsTargetDefended, WorldStateDefiner.None);
    }

    protected override void StartAction()
    {
        SetPositions();
    }

    protected override void UpdateAction()
    {
        if (Time.time >= stopTime) 
        {
            bIsDone = true;
        }
    }

    public override void UnpauseAction(Unit unit)
    {
        SetPositions();
    }

    private void SetPositions()
    {
        float baseAngle = Mathf.PI * 2f / squad.Units.Count;
        for (int i = 0; i < squad.Units.Count; ++i)
        {
            float angle = i * baseAngle;
            squad.Units[i].NavMeshAgent.SetDestination(squad.CurrentTarget.PositionTarget + new Vector3(Mathf.Cos(angle) * radius, squad.Units[i].transform.position.y, Mathf.Sin(angle) * radius));
            squad.Units[i].NavMeshAgent.isStopped = false;
        }

        stopTime = Time.time + defendTime;
    }
}
