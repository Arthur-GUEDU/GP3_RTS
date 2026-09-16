using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

[Serializable]
public class GoapPlanner
{
    private WorldState m_worldState;
    [HideInInspector] public List<GoapAction> actionSet = new List<GoapAction>();
    public bool isForward = true;
    public bool isDebugEnabled = false;

    private Stopwatch m_stopwatch = new Stopwatch();

    public WorldState WorldState { get { return m_worldState; } }

    public void SetWorldState(WorldStateDefiner _positiveWorldState, WorldStateDefiner _negativeWorldState)
    {
        m_worldState = new WorldState(_positiveWorldState, _negativeWorldState);
    }

    public void SetWorldState(WorldStateDefiner _positiveWorldState)
    {
        m_worldState = new WorldState(_positiveWorldState, ~_positiveWorldState);
    }

    public void ApplyActionEffects(WorldState _effects)
    {
        m_worldState = m_worldState.ApplyWorldState(_effects);
    }

    public List<GoapAction> GeneratePlan(GoapGoal _goal)
    {
        m_stopwatch.Reset();
        m_stopwatch.Start();

        GoapGoal goal = ScriptableObject.CreateInstance<GoapGoal>();
        goal.goalState = _goal.goalState;
        Node firstNode;

        List<GoapAction> plan = new List<GoapAction>();
        List<Node> leaves = new List<Node>();

        if (isForward)
        {
            firstNode = new Node(null, null, m_worldState);
            BuildForwardGraph(firstNode, leaves, actionSet, goal);
        }
        else
        {
            firstNode = new Node(null, null, goal.ApplyGoal(m_worldState));
            BuildBackwardGraph(firstNode, leaves, actionSet, m_worldState);
        }

        float minCost = float.MaxValue;
        int bestLeafIndex = -1;
        for (int i = 0; i < leaves.Count; ++i)
        {
            if (leaves[i].costSoFar < minCost)
            {
                minCost = leaves[i].costSoFar;
                bestLeafIndex = i;
            }
        }
        if (bestLeafIndex != -1)
            leaves[bestLeafIndex].ReconstructPlan(plan, isForward);

        m_stopwatch.Stop();

        if (isDebugEnabled)
        {
            UnityEngine.Debug.Log("Plan Generation took " + m_stopwatch.Elapsed.TotalMilliseconds + " ms");
            DebugPlan(plan);
        }

        return plan;
    }

    void BuildForwardGraph(Node _parent, List<Node> _leaves, List<GoapAction> _availableActions, GoapGoal _goal)
    {
        foreach (GoapAction action in _availableActions)
        {
            if (action.CheckPreconditions(_parent.worldState)) // check preconditions
            {
                WorldState newWorldState = action.ApplyEffects(_parent.worldState);
                Node node = new Node(action, _parent, newWorldState);
                if (_goal.IsFulfilled(newWorldState))
                {
                    _leaves.Add(node);
                }
                else
                {
                    // used action is removed
                    List<GoapAction> newAvalaibleActions = new List<GoapAction>(_availableActions);
                    newAvalaibleActions.Remove(action);
                    // recursive call on the new node
                    BuildForwardGraph(node, _leaves, newAvalaibleActions, _goal);
                }
            }
        }
    }

    void BuildBackwardGraph(Node _child, List<Node> _roots, List<GoapAction> _availableActions, WorldState _initialWorldState)
    {
        foreach (GoapAction action in _availableActions) // test each remaining action
        {
            if (!action.CheckEffects(_child.worldState)) // no useful effect, we skip
                continue;
            WorldState regressedWorldState = action.RegressGoal(_child.worldState); // create sub-goal
            if (!action.CheckPreconditions(regressedWorldState)) //preconditions cannot be satisfied on sub-goal, we skip
                continue;

            Node node = new Node(action, _child, regressedWorldState);
            if (regressedWorldState.LooseCompare(_initialWorldState)) // we reached initial state !
            {
                _roots.Add(node); // store root
            }
            else
            {
                // used action is removed
                List<GoapAction> newAvalaibleActions = new List<GoapAction>(_availableActions);
                newAvalaibleActions.Remove(action);
                // recursive call on the new node
                BuildBackwardGraph(node, _roots, newAvalaibleActions, _initialWorldState);
            }
        }
    }

    void DebugPlan(List<GoapAction> _plan)
    {
        string modeString = isForward ? "(Forward) : " : "(Backward) : ";
        string planString = "Plan " + modeString;
        foreach (GoapAction action in _plan)
            planString += action.actionName + " - ";
        planString += "Goal Reached !";
        UnityEngine.Debug.Log(planString);
    }
}

public class Node
{
    public GoapAction action;
    public Node parentNode;
    public float costSoFar;
    public WorldState worldState;


    public Node(GoapAction _action, Node _parent, WorldState _worldState)
    {
        action = _action;
        parentNode = _parent;
        worldState = _worldState;
        if (_parent != null && _action != null)
            costSoFar = _parent.costSoFar + _action.GetCost(worldState);
    }

    public void ReconstructPlan(List<GoapAction> _currPlan, bool isForward)
    {
        if (action == null || parentNode == null)
            return;

        if (isForward)
            _currPlan.Insert(0, action);
        else
            _currPlan.Add(action);
        parentNode.ReconstructPlan(_currPlan, isForward);
    }
}