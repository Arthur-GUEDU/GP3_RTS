using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GOAPGoal", menuName = "GOAP/Goal")]
public class GoapGoal : ScriptableObject
{
    public string goalName;
    public WorldState goalState;
    public bool needsAgressiveness;

    public GoapGoal(string _goalName, WorldStateDefiner _positiveGoalState, WorldStateDefiner _negativeGoalState)
    {
        goalName = _goalName;
        goalState = new WorldState(_positiveGoalState, _negativeGoalState);
    }

    public GoapGoal(GoapGoal _goal)
    {
        goalState = new WorldState(_goal.goalState);
    }

    public bool IsFulfilled(WorldState _worldState)
    {
        return _worldState.LooseCompare(goalState);
    }

    public WorldState ApplyGoal(WorldState _worldState)
    {
        return _worldState.ApplyWorldState(goalState);
    }
}
