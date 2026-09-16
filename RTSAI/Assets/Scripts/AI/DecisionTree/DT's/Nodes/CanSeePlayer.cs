using UnityEngine;

public class CanSeePlayer : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        if (controller.playerFactorys.Count > 0)
        {
            return true;
        }
        return false;
    }
}
