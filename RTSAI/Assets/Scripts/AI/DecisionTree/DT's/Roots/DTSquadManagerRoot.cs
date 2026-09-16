using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class DTSquadManagerRoot : DecisionTreeRoot
{
    public Squad joblessSquad;

    public void OnSquadNeedObjective(Squad squad)
    {
        joblessSquad = squad;
        Evaluate();
    }
}
