using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum ETeam
{
    Blue = 0,
    Red = 1,

    Neutral
}

[RequireComponent(typeof(GameState))]
public class GameServices : MonoBehaviour
{
    [SerializeField, Tooltip("Generic material used for 3D models, in the following order : blue, red and green")]
    Material[] TeamMaterials = new Material[3];

    [SerializeField, Tooltip("Unplayable terrain border size")]
    float NonPlayableBorder = 100f;

    [SerializeField, Tooltip("Playable bounds size if no terrain is found")]
    float DefaultPlayableBoundsSize = 100f;

    [SerializeField, Tooltip("Prefab used for the instanciation of Squads")]
    Squad SquadPrefab;

    static GameServices Instance = null;

    UnitController[] ControllersArray;
    TargetBuilding[] TargetBuildingArray;
    GameState CurrentGameState = null;

    Terrain CurrentTerrain = null;
    Bounds PlayableBounds;

    #region Static methods
    public static GameServices GetGameServices()
    {
        return Instance;
    }
    public static GameState GetGameState()
    {
        return Instance.CurrentGameState;
    }
    public static UnitController GetControllerByTeam(ETeam team)
    {
        if (Instance.ControllersArray.Length < (int)team)
            return null;
        return Instance.ControllersArray[(int)team];
    }
    public static Material GetTeamMaterial(ETeam team)
    {
        return Instance.TeamMaterials[(int)team];
    }
    public static ETeam GetOpponent(ETeam team)
    {
        return Instance.CurrentGameState.GetOpponent(team);
    }

    public static TargetBuilding[] GetTargetBuildings() { return Instance.TargetBuildingArray; }

    // return RGB color struct for each team
    public static Color GetTeamColor(ETeam team)
    {
        switch(team)
        {
            case ETeam.Blue:
                return Color.blue;
            case ETeam.Red:
                return Color.red;
            default:
                return Color.grey;
        }
    }
    public static float GetNonPlayableBorder { get { return Instance.NonPlayableBorder; } }
    public static Terrain GetTerrain { get { return Instance.CurrentTerrain; } }
    public static Bounds GetPlayableBounds()
    {
        return Instance.PlayableBounds;
    }
    public static Vector3 GetTerrainSize()
    {
        return Instance.TerrainSize;
    }
    public static Squad GetSquadPrefab()
    {
        return Instance.SquadPrefab;
    }
    public static bool IsPosInPlayableBounds(Vector3 pos)
    {
        if (GetPlayableBounds().Contains(pos))
            return true;

        return false;
    }
    public Vector3 TerrainSize
    {
        get
        {
            if (CurrentTerrain)
                return CurrentTerrain.terrainData.bounds.size;
            return new Vector3(DefaultPlayableBoundsSize, 10.0f, DefaultPlayableBoundsSize);
        }
    }

    public static List<GoapAction> GetActionList()
    {
        List<GoapAction> actionList = new List<GoapAction>();
        actionList.Add(new AttackAction());
        actionList.Add(new RepairAction());
        actionList.Add(new DefendAction());
        actionList.Add(new CaptureAction());
        actionList.Add(new MoveToAction());
        return actionList;
    }

    public static Unit GetClosestUnitToObjective(List<Unit> _units, Vector3 _objective)
    {
        float minDist = float.MaxValue;
        Unit closestUnit = null;

        foreach (Unit unit in _units)
        {
            float dist = Vector3.Distance(unit.transform.position, _objective);
            if (dist < minDist)
            {
                closestUnit = unit;
                minDist = dist;
            }
        }

        return closestUnit;
    }

    public static float GetLowestSpeedInUnitList(List<Unit> _units)
    {
        if (_units.Count == 0)
            return 0f;

        float minSpeed = _units[0].GetUnitData.Speed;

        for (int i = 1; i < _units.Count; ++i)
        {
            float speed = _units[i].GetUnitData.Speed;

            if (speed < minSpeed)
                minSpeed = speed;
        }

        return minSpeed;
    }

    #endregion

    #region MonoBehaviour methods
    void Awake()
    {
        Instance = this;

        // Retrieve controllers from scene for each team
        ControllersArray = new UnitController[2];
        foreach (UnitController controller in FindObjectsByType<UnitController>(FindObjectsSortMode.None))
        {
            ControllersArray[(int)controller.GetTeam()] = controller;
        }

        // Store TargetBuildings
        TargetBuildingArray = FindObjectsByType<TargetBuilding>(FindObjectsSortMode.None);

        // Store GameState ref
        if (CurrentGameState == null)
            CurrentGameState = GetComponent<GameState>();

        // Assign first found terrain
        CurrentTerrain = FindFirstObjectByType<Terrain>();
        if (CurrentTerrain)
        {
            PlayableBounds = CurrentTerrain.terrainData.bounds;
            Vector3 clampedOne = new Vector3(1f, 0f, 1f);
            Vector3 heightReduction = Vector3.up * 0.1f; // $$ hack : this is to prevent selectioning / building in high areas
            PlayableBounds.SetMinMax(PlayableBounds.min + clampedOne * NonPlayableBorder / 2f, PlayableBounds.max - clampedOne * NonPlayableBorder / 2f - heightReduction);
        }
        else
        {
            Debug.LogWarning("could not find terrain asset in scene, setting default PlayableBounds");
            Vector3 clampedOne = new Vector3(1f, 0f, 1f);
            PlayableBounds.SetMinMax(   new Vector3(-DefaultPlayableBoundsSize, -10.0f, -DefaultPlayableBoundsSize) + clampedOne * NonPlayableBorder / 2f,
                                        new Vector3(DefaultPlayableBoundsSize, 10.0f, DefaultPlayableBoundsSize) - clampedOne * NonPlayableBorder / 2f);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(PlayableBounds.center, PlayableBounds.size);
    }
    #endregion
}
