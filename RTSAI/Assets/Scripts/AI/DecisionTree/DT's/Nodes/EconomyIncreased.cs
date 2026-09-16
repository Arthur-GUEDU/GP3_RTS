using UnityEngine;

public class EconomyIncreased : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;
    public int currentEconomy = 0;

    public override bool IsConditionValid()
    {
        if (controller.OverallBuildPoints <= currentEconomy)
        {
            return false;
        }

        currentEconomy = controller.OverallBuildPoints;
        return true;
    }
}
