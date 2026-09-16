using UnityEngine;

public enum EFactoryWeightClass
{
    Light = 0,
    Heavy,
}

[CreateAssetMenu(fileName = "Factory_Data", menuName = "RTS/FactoryData", order = 1)]
public class FactoryDataScriptable : EntityDataScriptable
{
    [Header("Classification")]
    public EFactoryWeightClass WeightClass = EFactoryWeightClass.Light;

    [Header("Spawn Unit Settings")]
    public int NbSpawnSlots = 10;
    public int SpawnRadius = 12;
    public int RadiusOffset = 4;

    [Header("Available Entities")]
    public GameObject[] AvailableUnits = null;
    public GameObject[] AvailableFactories = null;

    [Header("FX")]
    public GameObject DeathFXPrefab = null;
}