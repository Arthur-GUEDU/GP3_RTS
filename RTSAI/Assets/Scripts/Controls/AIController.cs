using System.Collections.Generic;
using UnityEngine;

public sealed class AIController : UnitController
{
    public DecisionTreeRoot economyRoot;
    public DTSquadManagerRoot squadManagerRoot;

    public List<Objective> objectives = new List<Objective>();

    [SerializeField] private List<UnitDataScriptable> scriptableUnits = new List<UnitDataScriptable>();

    public int stranghtCap = 0;

    public List<GameObject> playerFactorys = new List<GameObject>();

    #region AI

    public void AddObjective(Objective _objective)
    {
        int index = 0;
        while (index < objectives.Count && objectives[index].Priority >= _objective.Priority)
            index++;

        objectives.Insert(index, _objective);
    }

    public void AssignBestObjective(Squad _squad)
    {
        if (_squad == null)
            return;

        Objective objective = objectives[0];
        if (objective == null)
            return;

        _squad.UpdateObjective(objective.Goal, objective.Target);
        objectives.Remove(objective);
    }

    public void AssignGoal(Squad _squad, GoapGoal _goal, Target _target)
    {
        if (_squad == null)
            return;

        _squad.UpdateObjective(_goal, _target);
    }

    public void BuildFactory(EFactoryWeightClass weightClass)
    {
        float ringStep = 5f;
        float maxRadius = 600f;
        int anglesPerRing = 12;

        foreach (Factory factory in GetFactoryList)
        {
            if (factory.CurrentState != Factory.State.Available)
                continue;

            for (int i = 0; i < factory.AvailableFactoriesCount; i++)
            {
                FactoryDataScriptable data = factory.GetBuildableFactoryData(i);
                if (data == null || data.WeightClass != weightClass)
                    continue;

                SelectFactory(factory);

                for (float radius = ringStep; radius <= maxRadius; radius += ringStep)
                {
                    for (int a = 0; a < anglesPerRing; a++)
                    {
                        float angle = a * (2f * Mathf.PI / anglesPerRing);
                        Vector3 offset = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
                        Vector3 buildPos = factory.transform.position + offset;

                        if (RequestFactoryBuild(i, buildPos))
                            return;
                    }
                }
            }
        }

        Debug.LogWarning($"AIController: Unable to build a {weightClass} factory. No valid position found.");
    }

    public int GetSquadOptimalSize()
    {
        return 4;
    }

    public void ReinforceSquad(Squad _squad)
    {
        if (_squad == null)
            return;

        int unitsNeeded = GetSquadOptimalSize() - _squad.Units.Count;

        for (int i = 0; i <= unitsNeeded; i++)
        {
            if (!TryBuildBestAffordableUnit(_squad))
            {
                break;
            }
        }
    }

    private bool TryBuildBestAffordableUnit(Squad _squad)
    {
        List<UnitDataScriptable> candidates = new List<UnitDataScriptable>(scriptableUnits);
        candidates.Sort((a, b) => b.Cost.CompareTo(a.Cost));

        foreach (UnitDataScriptable unitData in candidates)
        {
            if (unitData == null || unitData.Cost > TotalBuildPoints)
                continue;

            if (TryRequestUnitAtAnyFactory(unitData, _squad))
                return true;
        }

        return false;
    }

    private bool TryRequestUnitAtAnyFactory(UnitDataScriptable _unitData, Squad _squad)
    {
        foreach (Factory factory in GetFactoryList)
        {
            if (factory.IsUnderConstruction)
                continue;

            for (int unitIndex = 0; unitIndex < factory.AvailableUnitsCount; unitIndex++)
            {
                if (factory.GetBuildableUnitData(unitIndex) != _unitData)
                    continue;

                SelectFactory(factory);

                if (RequestUnitBuild(unitIndex, (Unit builtUnit) =>
                {
                    if (builtUnit != null)
                        _squad.AddUnit(builtUnit);
                }))
                    return true;

                break;
            }
        }

        return false;
    }

    public Squad SpawnAISquad()
    {
        Squad newSquad = Instantiate<Squad>(GameServices.GetSquadPrefab(), FactoryList[0].transform.position, FactoryList[0].transform.rotation);

        newSquad.Controller = this;
        newSquad.Planner.actionSet = GameServices.GetActionList();
        newSquad.Planner.SetWorldState(WorldStateDefiner.None);
        newSquad.Team = Team;
        SquadList.Add(newSquad);
        newSquad.OnPlanEnd.AddListener(squadManagerRoot.OnSquadNeedObjective);

        newSquad.OnPlanEnd.Invoke(newSquad);
        return newSquad;
    }

    #endregion

    #region MonoBehaviour methods

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        economyRoot = FindAnyObjectByType<DTEconomyRoot>();
        squadManagerRoot = FindAnyObjectByType<DTSquadManagerRoot>();

        if (economyRoot != null)
        {
            if (OnBuildPointsUpdated != null)
                OnBuildPointsUpdated -= economyRoot.Evaluate;
            OnBuildPointsUpdated += economyRoot.Evaluate;
        	OnBuildPointsUpdated.Invoke();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.B))
        {
            SelectFactory(FactoryList[0]);
            RequestUnitBuild(0);
        }

        if (Input.GetKeyUp(KeyCode.A))
            TotalBuildPoints += 15;
    }

    #endregion
}