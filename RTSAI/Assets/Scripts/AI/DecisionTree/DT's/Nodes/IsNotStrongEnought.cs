using UnityEngine;

public class IsNotStrongEnought : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        if (controller.OverallBuildPoints > controller.stranghtCap)
        {
            return false;
        }
        return true;
    }
}
