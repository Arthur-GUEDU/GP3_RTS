using UnityEngine;

public class CreateNewSquad : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;

    public override void Execute()
    {
        Squad s = controller.SpawnAISquad();

        controller.ReinforceSquad(s);
    }
}
