using UnityEngine;

public class ThereIsObjective : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        if (controller.objectives.Count <= 0)
        {
            return false;
        }
        return true;
    }
}
