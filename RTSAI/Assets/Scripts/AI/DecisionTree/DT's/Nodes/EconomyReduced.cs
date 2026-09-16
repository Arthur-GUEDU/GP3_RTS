using UnityEngine;

public class EconomyReduced : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;
    public int currentEconomy = 0;

    public override bool IsConditionValid()
    {
        if (controller.TotalBuildPoints <= currentEconomy)
        {
            currentEconomy = controller.TotalBuildPoints;
            return true;
        }

        return false;
    }
}
