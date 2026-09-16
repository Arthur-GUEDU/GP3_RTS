using UnityEngine;

[System.Serializable]
public class Transition
{
    #region Serialize Field Variables
    [SerializeField] private IStateBehaviour m_nextState;
    [SerializeField] private ICondition[] m_conditions;
    #endregion

    #region Variables
    ICondition[] m_conditionsInst;
    #endregion

    #region Getter / Setter
    public IStateBehaviour NextState => m_nextState;
    #endregion

    #region Methods
    /// <summary>
    /// Init Transition by instantiate conditions
    /// </summary>
    public void Init()
    {
        m_conditionsInst = new ICondition[m_conditions.Length];
        for (int i = 0; i < m_conditions.Length; ++i)
        {
            m_conditionsInst[i] = Object.Instantiate(m_conditions[i]);
        }
    }

    /// <summary>
    /// Check if conditions are fulfilled
    /// </summary>
    /// <param name="_blackBoard"></param>
    /// <param name="_owner"></param>
    /// <returns>True if transition is possible, else False</returns>
    public bool IsValid(BlackBoard _blackBoard, GameObject _owner)
    {
        foreach (ICondition cond in m_conditionsInst)
        {
            if (cond == null)
                continue;

            if (!cond.IsValid(_blackBoard, _owner))
                return false;
        }
        return true;
    }
    #endregion
}