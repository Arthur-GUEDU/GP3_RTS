using System;

[Flags]
public enum WorldStateDefiner : int
{
    None = 0,               // 00000
    IsAtLocation = 1,       // 00001
    IsTargetCaptured = 2,   // 00010
    IsTargetDestroyed = 4,  // 00100
    IsTargetDefended = 8,   // 01000
    IsTargetRepaired = 16,  // 10000
}

[Serializable]
public class WorldState
{
    /// <summary>
    /// True definers for this world state
    /// </summary>
    public WorldStateDefiner positiveState;
    /// <summary>
    /// False definers for this world state
    /// </summary>
    public WorldStateDefiner negativeState;

    public WorldState(WorldStateDefiner _positiveState, WorldStateDefiner _negativeState)
    {
        positiveState = _positiveState;
        negativeState = _negativeState;
    }

    public WorldState(WorldState _worldState)
    {
        positiveState = _worldState.positiveState;
        negativeState = _worldState.negativeState;
    }

    public WorldState ApplyWorldState(WorldState _worldState)
    {
        WorldStateDefiner notPositive = ~_worldState.positiveState;
        WorldStateDefiner notNegative = ~_worldState.negativeState;
        return new WorldState((positiveState | _worldState.positiveState) & notNegative, (negativeState | _worldState.negativeState) & notPositive);
    }

    /// <summary>
    /// Checks if the first WorldState contains the second WorldState
    /// </summary>
    /// <param name="worldState"></param>
    /// <returns></returns>
    public bool LooseCompare(WorldState worldState)
    {
        return ((positiveState & worldState.positiveState) == worldState.positiveState) && ((negativeState & worldState.negativeState) == worldState.negativeState);
    }

    /// <summary>
    /// Strictly compares the 2 WorldStates
    /// </summary>
    /// <param name="worldState"></param>
    /// <returns></returns>
    public bool Compare(WorldState worldState)
    {
        return (positiveState == worldState.positiveState) && (negativeState == worldState.negativeState);
    }

    bool IsValid()
    {
        return true; // positive and negative are not countering each others
    }
}
