using UnityEngine;

public class ObjectiveNotInVision : DecisionTreeNode
{
    public override bool IsConditionValid()
    {
        return true;
    }
}
