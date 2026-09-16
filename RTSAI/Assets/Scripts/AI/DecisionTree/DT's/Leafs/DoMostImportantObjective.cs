using UnityEngine;

public class DoMostImportantObjective : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;
    public DTSquadManagerRoot root;


    public override void Execute()
    {
        controller.AssignBestObjective(root.joblessSquad);
    }
}
