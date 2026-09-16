using UnityEngine;

public class AttackPlayer : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;

    public DTSquadManagerRoot squadManagerRoot;

    public override void Execute()
    {
        controller.AssignGoal(squadManagerRoot.joblessSquad, controller.DestroyGoal, new Target(controller.playerFactorys[0].GetComponent<BaseEntity>()));
    }
}
