using UnityEngine;

public class NotEnoughtLightFactory : DecisionTreeNode
{
    [Header("Conditions")]
    public AIController controller;

    public override bool IsConditionValid()
    {
        int heavyFactoryCount = 0;
        int liquidFactoryCount = 0;
        foreach (Factory factory in controller.GetFactoryList)
        {
            if (factory.WeightClass == EFactoryWeightClass.Heavy)
            {
                heavyFactoryCount++;
            }

            if (factory.WeightClass == EFactoryWeightClass.Light)
            {
                liquidFactoryCount++;
            }
        }

        if (liquidFactoryCount < heavyFactoryCount)
        {
            return true;
        }
        return false;
    }
}
