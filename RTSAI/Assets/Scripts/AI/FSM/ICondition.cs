using UnityEngine;

public class ICondition : ScriptableObject
{
    [SerializeField] protected bool bIsInverted = false;
 
    /// <summary>
    /// Check if the condition is valid
    /// </summary>
    /// <param name="_blackBoard"></param>
    /// <param name="_owner"></param>
    /// <returns>True if condition is valid, else False</returns>
    public virtual bool IsValid(BlackBoard _blackBoard, GameObject _owner)
    {
        return !bIsInverted;
    }
}