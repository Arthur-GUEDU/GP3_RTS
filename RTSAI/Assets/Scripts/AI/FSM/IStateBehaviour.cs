using UnityEngine;

[System.Serializable]
public class IStateBehaviour : ScriptableObject
{
    #region Private Variables
    [SerializeField] private Transition[] m_transitions = null;
    #endregion

    #region Getter/Setter
    public Transition[] Transitions { get { return m_transitions; } }
    #endregion

    #region Methods
    /// <summary>
    /// Init all transitions
    /// </summary>
    public void Init()
    {
        for (int i = 0; i < m_transitions.Length; i++)
            m_transitions[i].Init();
    }
    #endregion

    #region Actions Events
    /// <summary>
    /// OnEnter event is call when the state start
    /// </summary>
    /// <param name="_blackBoard"></param>
    /// <param name="_owner"></param>
    public virtual void OnEnter(ref BlackBoard _blackBoard, GameObject _owner) { }
    /// <summary>
    /// OnUpdate event is call each frame
    /// </summary>
    /// <param name="_blackBoard"></param>
    /// <param name="_owner"></param>
    public virtual void OnUpdate(ref BlackBoard _blackBoard, GameObject _owner) { }
    /// <summary>
    /// OnExit event is call at the end of the state before switch to another state
    /// </summary>
    /// <param name="_blackBoard"></param>
    /// <param name="_owner"></param>
    public virtual void OnExit(ref BlackBoard _blackBoard, GameObject _owner) { }
    #endregion
}