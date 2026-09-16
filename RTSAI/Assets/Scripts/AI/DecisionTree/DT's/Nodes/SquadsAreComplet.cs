using UnityEngine;

public class SquadsAreComplet : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        for (int i = 0; i < controller.GetSquadList.Count; i++)
        {
            if (controller.GetSquadList[i].Units.Count <= controller.GetSquadOptimalSize())
            {
                return false;
            }
        }
        return true;
    }
}
