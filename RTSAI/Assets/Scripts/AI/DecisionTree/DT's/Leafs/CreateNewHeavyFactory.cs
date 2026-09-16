using UnityEngine;

public class CreateNewHeavyFactory : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;

    public override void Execute()
    {
        controller.BuildFactory(EFactoryWeightClass.Heavy);
    }
}
