using UnityEngine;

public class ShouldBuildFactory : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        if (controller.FactoriesSpentBuildPoints * 2 < controller.UnitsSpentBuildPoints && controller.TotalBuildPoints > 0)
        {
            return true;
        }

        return false;
    }
}
