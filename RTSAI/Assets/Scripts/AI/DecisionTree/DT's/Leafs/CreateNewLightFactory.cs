using UnityEngine;

public class CreateNewLightFactory : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;

    public override void Execute()
    {
        controller.BuildFactory(EFactoryWeightClass.Light);
    }
}
