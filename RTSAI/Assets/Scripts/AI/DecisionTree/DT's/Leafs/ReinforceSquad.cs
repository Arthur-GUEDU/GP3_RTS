using UnityEngine;

public class ReinforceSquad : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;

    public override void Execute()
    {
        for (int i = 0; i < controller.GetSquadList.Count; i++)
        {
            if (controller.GetSquadList[i].Units.Count < controller.GetSquadOptimalSize())
            {
                controller.ReinforceSquad(controller.GetSquadList[i]);
            }
        }

        if (controller.TotalBuildPoints > 0)
        {
            Squad s = controller.SpawnAISquad();
            controller.ReinforceSquad(s);
        }
    }
}
