using UnityEngine;

[System.Serializable]
public class Objective //:
{
    [SerializeField] private GoapGoal m_goal;
    [SerializeField] private Target m_target;
    [SerializeField] private int m_priority;

    public GoapGoal Goal { get { return m_goal; } }
    public Target Target { get { return m_target; } }
    public int Priority { get { return m_priority; } }

    public Objective(GoapGoal goal, Target target, int priority)
    {
        m_goal = goal;
        m_target = target;
        m_priority = priority;
    }
}