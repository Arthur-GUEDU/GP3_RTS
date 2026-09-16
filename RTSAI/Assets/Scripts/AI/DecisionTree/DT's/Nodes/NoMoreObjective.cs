using UnityEngine;

public class NoMoreObjective : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        if (controller.objectives.Count <= 0)
        {
            return true;
        }
        return false;
    }
}
