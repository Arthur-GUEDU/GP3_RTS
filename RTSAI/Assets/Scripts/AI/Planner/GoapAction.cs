using UnityEngine;

public class GoapAction
{
    public string actionName;
    public WorldState preconditions;
    public WorldState effects;
    public float cost;

    protected Squad squad;
    protected bool bHasStarted;
    protected bool bIsDone;

    public bool HasStarted { get { return bHasStarted; } }
    public bool IsDone { get { return bIsDone; } }

    public void OnStart(Squad _squad) 
    {
        squad = _squad;
        bHasStarted = true;
        StartAction();
    }

    protected virtual void StartAction() {}

    public void OnUpdate()
    {
        UpdateAction();
    }

    protected virtual void UpdateAction() {}

    public void OnEnd()
    {
        EndAction();
        squad.Planner.ApplyActionEffects(effects);
        Reset();
    }

    protected virtual void EndAction() {}

    private void Reset()
    {
        bHasStarted = false;
        bIsDone = false;
        squad = null;
    }

    public virtual void PauseAction(Unit unit) {}
    public virtual void UnpauseAction(Unit unit) {}

    public void SetPreconditions(WorldStateDefiner _positivePreconditions, WorldStateDefiner _negativePreconditions)
    {
        preconditions = new WorldState(_positivePreconditions, _negativePreconditions);
    }

    public void SetEffects(WorldStateDefiner _positiveEffects, WorldStateDefiner _negativeEffects)
    {
        effects = new WorldState(_positiveEffects, _negativeEffects);
    }

    /// <summary>
    /// Can be overriden to add a cost depending on world state
    /// </summary>
    /// <param name="_worldState"></param>
    /// <returns></returns>
    public virtual float GetCost(WorldState _worldState)
    {
        return cost;
    }

    public bool CheckPreconditions(WorldState _worldState)
    {
        return _worldState.LooseCompare(preconditions);
    }

    public bool CheckEffects(WorldState _worldState)
    {

        return _worldState.LooseCompare(effects);
    }

    public WorldState ApplyEffects(WorldState _worldState)
    {
        return _worldState.ApplyWorldState(effects);
    }

    public WorldState RegressGoal(WorldState _worldState)
    {
        return _worldState.ApplyWorldState(new WorldState(effects.negativeState, effects.positiveState)).ApplyWorldState(preconditions);
    }
}
