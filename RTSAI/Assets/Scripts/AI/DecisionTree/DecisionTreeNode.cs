using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionTreeNode : MonoBehaviour
{
    public List<DecisionTreeNode> childrens = new List<DecisionTreeNode>();
    public DecisionTreeLeaf leaf;

    public bool Evaluate()
    {
        if (!IsConditionValid())
            return false;

        if (leaf != null)
        {
            leaf.Execute();
            return true;
        }

        foreach (DecisionTreeNode child in childrens)
        {
            if (child.Evaluate())
                return true;
        }

        return false;
    }

    public abstract bool IsConditionValid();

}
