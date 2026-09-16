using System;
using System.Collections.Generic;
using UnityEngine;

// points system for units creation (Ex : light units = 1 pt, medium = 2pts, heavy = 3 pts)
// max points can be increased by capturing TargetBuilding entities
public class UnitController : MonoBehaviour
{
    [SerializeField]
    protected ETeam Team;
    public ETeam GetTeam() { return Team; }

    [SerializeField]
    protected int StartingBuildPoints = 15;

    protected int _TotalBuildPoints = 0;
    public int TotalBuildPoints
    {
        get { return _TotalBuildPoints; }
        set
        {
            Debug.Log("TotalBuildPoints updated");
            _TotalBuildPoints = value;
            OverallBuildPoints = TotalBuildPoints + UnitsSpentBuildPoints + FactoriesSpentBuildPoints;
            OnBuildPointsUpdated?.Invoke();
        }
    }

    protected int _OverallBuildPoints = 0;
    public int OverallBuildPoints
    {
        get { return _OverallBuildPoints; }
        set
        {
            Debug.Log("OverallBuildPoints updated");
            _OverallBuildPoints = value;
        }
    }

    protected int _UnitsSpentBuildPoints = 0;
    public int UnitsSpentBuildPoints
    {
        get { return _UnitsSpentBuildPoints; }
        set
        {
            Debug.Log("UnitsSpentBuildPoints updated");
            _UnitsSpentBuildPoints = value;
            OverallBuildPoints = TotalBuildPoints + UnitsSpentBuildPoints + FactoriesSpentBuildPoints;
        }
    }

    protected int _FactoriesSpentBuildPoints = 0;
    public int FactoriesSpentBuildPoints
    {
        get { return _FactoriesSpentBuildPoints; }
        set
        {
            Debug.Log("FactoriesSpentBuildPoints updated");
            _FactoriesSpentBuildPoints = value;
            OverallBuildPoints = TotalBuildPoints + UnitsSpentBuildPoints + FactoriesSpentBuildPoints;
        }
    }

    protected int _CapturedTargets = 0;
    public int CapturedTargets
    {
        get { return _CapturedTargets; }
        set
        {
            _CapturedTargets = value;
            OnCaptureTarget?.Invoke();
        }
    }

    protected Transform TeamRoot = null;
    public Transform GetTeamRoot() { return TeamRoot; }

    public List<Unit> UnitList
    {
        get;
        protected set;
    }
    protected List<Unit> SelectedUnitList = new List<Unit>();
    protected List<Factory> FactoryList = new List<Factory>();
    protected List<Squad> SquadList = new List<Squad>();

    public List<Factory> GetFactoryList { get { return FactoryList; } }
    protected Factory SelectedFactory = null;

    public List<Squad> GetSquadList { get { return SquadList; } }
    protected Squad SelectedSquad = null;

    // events
    protected Action OnBuildPointsUpdated;
    protected Action OnCaptureTarget;

    // GOAPGoals
    [SerializeField] public GoapGoal DestroyGoal;
    [SerializeField] public GoapGoal DefendGoal;
    [SerializeField] public GoapGoal RepairGoal;
    [SerializeField] public GoapGoal CaptureGoal;
    [SerializeField] public GoapGoal ExploreGoal;

    #region Unit methods
    protected void UnselectAllUnits()
    {
        foreach (Unit unit in SelectedUnitList)
            unit.SetSelected(false);
        SelectedUnitList.Clear();
    }
    protected void SelectAllUnits()
    {
        foreach (Unit unit in UnitList)
            unit.SetSelected(true);

        SelectedUnitList.Clear();
        SelectedUnitList.AddRange(UnitList);
    }
    protected void SelectAllUnitsByTypeId(int typeId)
    {
        UnselectCurrentFactory();
        UnselectAllUnits();
        UnselectSquad();
        SelectedUnitList = UnitList.FindAll(delegate (Unit unit)
        {
            return unit.GetTypeId == typeId;
        }
        );
        foreach (Unit unit in SelectedUnitList)
        {
            unit.SetSelected(true);
        }
    }
    protected void SelectUnitList(List<Unit> units)
    {
        foreach (Unit unit in units)
            unit.SetSelected(true);
        SelectedUnitList.AddRange(units);
    }
    protected void SelectUnitList(Unit[] units)
    {
        foreach (Unit unit in units)
            unit.SetSelected(true);
        SelectedUnitList.AddRange(units);
    }
    protected void SelectUnit(Unit unit)
    {
        unit.SetSelected(true);
        SelectedUnitList.Add(unit);
    }
    protected void UnselectUnit(Unit unit)
    {
        unit.SetSelected(false);
        SelectedUnitList.Remove(unit);
    }
    virtual public void AddUnit(Unit unit)
    {
        UnitsSpentBuildPoints += unit.Cost;
        unit.OnDeadEvent += () =>
        {
            TotalBuildPoints += unit.Cost;
            UnitsSpentBuildPoints -= unit.Cost;
            if (unit.IsSelected)
                SelectedUnitList.Remove(unit);
            UnitList.Remove(unit);
        };
        UnitList.Add(unit);
    }

    protected virtual void SelectSquad(Squad _squad)
    {
        if (_squad != null)
        {
            SelectedSquad = _squad;
            SelectedSquad.SetSelected(true);
        }
    }

    protected virtual void UnselectSquad()
    {
        if (SelectedSquad != null)
        {
            SelectedSquad.SetSelected(false);
            SelectedSquad = null;
        }
    }

    public void CaptureTarget(int points)
    {
        Debug.Log("CaptureTarget");
        TotalBuildPoints += points;
        OverallBuildPoints += points;
        CapturedTargets++;
    }
    public void LoseTarget(int points)
    {
        TotalBuildPoints -= points;
        OverallBuildPoints -= points;
        CapturedTargets--;
    }
    #endregion

    #region Squad methods
    protected void SpawnSquad(List<Unit> _units)
    {
        Squad newSquad = Instantiate<Squad>(GameServices.GetSquadPrefab(), _units[0].transform.position, _units[0].transform.rotation);
        for (int i = 0; i < _units.Count; ++i)
        {
            newSquad.AddUnit(_units[i]);
        }
        newSquad.Controller = this;
        // Set available actions and WorldState for the squad's planner
        newSquad.Planner.actionSet = GameServices.GetActionList();
        newSquad.Planner.SetWorldState(WorldStateDefiner.None);
        newSquad.Team = Team;
        newSquad.NavMeshAgent.speed = GameServices.GetLowestSpeedInUnitList(_units);
        SquadList.Add(newSquad);
        SelectSquad(newSquad);
    }

    public void DeleteSquad(Squad _squad)
    {
        for (int i = 0; i < SquadList.Count; ++i)
        {
            if (SquadList[i] == _squad)
            {
                SquadList.RemoveAt(i);
                Destroy(_squad.gameObject);
            }
        }
    }

    #endregion

    #region Factory methods
    void AddFactory(Factory factory)
    {
        if (factory == null)
        {
            Debug.LogWarning("Trying to add null factory");
            return;
        }

        FactoriesSpentBuildPoints += factory.Cost;
        factory.OnDeadEvent += () =>
        {
            TotalBuildPoints += factory.Cost;
            FactoriesSpentBuildPoints -= factory.Cost;
            if (factory.IsSelected)
                SelectedFactory = null;
            FactoryList.Remove(factory);
        };
        FactoryList.Add(factory);
    }
    virtual protected void SelectFactory(Factory factory)
    {
        if (factory == null || factory.IsUnderConstruction)
            return;

        SelectedFactory = factory;
        SelectedFactory.SetSelected(true);
        UnselectAllUnits();
        UnselectSquad();
    }
    virtual protected void UnselectCurrentFactory()
    {
        if (SelectedFactory != null)
            SelectedFactory.SetSelected(false);
        SelectedFactory = null;
    }
    protected bool RequestUnitBuild(int unitMenuIndex)
    {
        return RequestUnitBuild(unitMenuIndex, null);
    }
    protected bool RequestUnitBuild(int unitMenuIndex, Action<Unit> _onUnitBuilt)
    {
        if (SelectedFactory == null)
            return false;

        return SelectedFactory.RequestUnitBuild(unitMenuIndex, _onUnitBuilt);
    }
    protected bool RequestFactoryBuild(int factoryIndex, Vector3 buildPos)
    {
        if (SelectedFactory == null)
            return false;

        int cost = SelectedFactory.GetFactoryCost(factoryIndex);
        if (TotalBuildPoints < cost)
            return false;

        // Check if positon is valid
        if (SelectedFactory.CanPositionFactory(factoryIndex, buildPos) == false)
            return false;

        Factory newFactory = SelectedFactory.StartBuildFactory(factoryIndex, buildPos);
        if (newFactory != null)
        {
            AddFactory(newFactory);
            TotalBuildPoints -= cost;

            return true;
        }
        return false;
    }
    #endregion

    #region MonoBehaviour methods
    virtual protected void Awake()
    {
        UnitList = new List<Unit>();
        string rootName = Team.ToString() + "Team";
        TeamRoot = GameObject.Find(rootName)?.transform;
        if (TeamRoot)
            Debug.LogFormat("TeamRoot {0} found !", rootName);
    }
    virtual protected void Start()
    {
        CapturedTargets = 0;
        TotalBuildPoints += StartingBuildPoints;
        OverallBuildPoints = StartingBuildPoints;
        UnitsSpentBuildPoints = 0;
        FactoriesSpentBuildPoints = 0;

        // get all team factory already in scene
        Factory[] allFactories = FindObjectsByType<Factory>(FindObjectsSortMode.None);
        foreach (Factory factory in allFactories)
        {
            if (factory.GetTeam() == GetTeam())
            {
                AddFactory(factory);
            }
        }

        Debug.Log("found " + FactoryList.Count + " factory for team " + GetTeam().ToString());
    }
    virtual protected void Update()
    {

    }
    #endregion
}