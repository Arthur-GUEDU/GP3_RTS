using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Squad : MonoBehaviour, ISelectable
{
    private List<Unit> m_units = new List<Unit>();
    private GoapPlanner m_planner = new GoapPlanner();
    private Formation m_formation = new TriangleFormation();

    private List<GoapAction> m_currentPlan;
    private int m_currentPlanIndex = 0;

    private Target m_currentTarget;
    private NavMeshAgent m_agent;

    private UnitController m_controller;

    float updateFrequency = 0.1f;
    private float nextUpdateTime;

    public UnityEvent<Squad> OnPlanEnd = new UnityEvent<Squad>();

    public ETeam Team { get; set; }
    bool m_isSelected = false;

    public GoapPlanner Planner { get { return m_planner; } set { m_planner = value; } }
    public List<Unit> Units { get { return m_units; } }
    public Target CurrentTarget { get { return m_currentTarget; } }
    public UnitController Controller { get { return m_controller; } set { m_controller = value; } }
    public NavMeshAgent NavMeshAgent { get { return m_agent; } }

    #region ISelectable
    public ETeam GetTeam()
    {
        return Team;
    }

    public void SetSelected(bool selected)
    {
        m_isSelected = selected;
    }
    #endregion

    #region Methods
    public void UpdateObjective(GoapGoal _goal, Target _target)
    {
        // End current action and reset plan
        if (m_currentPlan != null && m_currentPlanIndex < m_currentPlan.Count)
        {
            m_currentPlan[m_currentPlanIndex].OnEnd();
            ResetPlan();
        }

        // Generate a new plan based on new goal and set the new target
        m_planner.SetWorldState(WorldStateDefiner.None);
        m_currentPlan = m_planner.GeneratePlan(_goal);
        m_currentTarget = _target;
        SetAgressiveness(_goal.needsAgressiveness);
    }

    public GoapAction GetCurrentAction()
    {
        if (m_currentPlan.Count == 0)
            return null;
        return m_currentPlan[m_currentPlanIndex];
    }

    public void ResetPlan()
    {
        m_currentPlan.Clear();
        m_currentPlanIndex = 0;
    }

    public void ResetWorldState()
    {
        m_planner.SetWorldState(WorldStateDefiner.None);
    }

    public void AddUnit(Unit _unit)
    {
        Units.Add(_unit);
        _unit.SetSquad(this);
    }

    public void RemoveUnit(Unit _unit)
    {
        Units.Remove(_unit);
        if (Units.Count == 0)
            Controller.DeleteSquad(this);
    }

    public void SetTargetPos(Vector3 _pos)
    {
        m_agent.SetDestination(_pos);
        m_agent.isStopped = false;
    }

    public void UpdateUnitsTargetPos()
    {
        List<Vector3> offsets = m_formation.GetFormationPositions(Units.Count, transform.rotation);
        for (int i = 0; i < Units.Count; ++i)
        {
            Units[i].SetTargetPos(transform.position + offsets[i]);
        }
    }

    public void SetAgressiveness(bool _isAgressive)
    {
        foreach (Unit unit in Units) 
        {
            unit.SetAgressive(_isAgressive);
        }
    }

    public bool Equals(List<Unit> _units)
    {
        int unitCount = 0;
        foreach (Unit unit in _units)
        {
            if (unit.Squad != null)
            {
                if (unit.Squad != this)
                    return false;
                else
                    ++unitCount;
            }
        }
        if (unitCount == Units.Count)
            return true;
        else
            return false;
    }

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        nextUpdateTime = Time.time + updateFrequency;
    }

    private void Update()
    {
        if (Time.time < nextUpdateTime)
            return;
        nextUpdateTime = Time.time + updateFrequency;

        if (m_currentPlan != null && m_currentPlan.Count > 0)
        {
            if (!m_currentPlan[m_currentPlanIndex].HasStarted)
                m_currentPlan[m_currentPlanIndex].OnStart(this);
            else
                m_currentPlan[m_currentPlanIndex].OnUpdate();

            if (m_currentPlan[m_currentPlanIndex].IsDone)
            {
                m_currentPlan[m_currentPlanIndex].OnEnd();
                ++m_currentPlanIndex;
                if (m_currentPlanIndex == m_currentPlan.Count)
                {
                    ResetWorldState();
                    ResetPlan();
                    OnPlanEnd.Invoke(this);
                }
            }
        }
    }

    #endregion
}