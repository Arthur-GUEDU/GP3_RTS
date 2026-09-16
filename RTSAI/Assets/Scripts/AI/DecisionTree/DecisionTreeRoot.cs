using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DecisionTreeRoot : MonoBehaviour
{
    public List<DecisionTreeNode> nodes = new List<DecisionTreeNode>();
    //public Action starterEvent;

    public void Evaluate()
    {
        foreach (DecisionTreeNode node in nodes)
        {
            if (node.Evaluate())
                return;
        }
    }
}
