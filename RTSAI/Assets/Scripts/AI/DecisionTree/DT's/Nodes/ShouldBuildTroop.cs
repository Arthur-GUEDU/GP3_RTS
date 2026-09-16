using UnityEngine;

public class ShouldBuildTroop : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        if (controller.UnitsSpentBuildPoints <= controller.FactoriesSpentBuildPoints * 2 && controller.TotalBuildPoints > 0)
        {
            return true;
        }

        return false;
    }
}
