using UnityEngine;

public class FSM : MonoBehaviour
{
    #region Serialize Field Variables
#if UNITY_EDITOR
    [ReadOnly, SerializeField] private IStateBehaviour m_crtState = null; // Read-Only for Debug in Editor
#else
    private IStateBehaviour m_crtState = null; // Read-Only for Debug in Editor
#endif
    [SerializeField] private IStateBehaviour m_initialState;
#endregion

    #region Variables
    [SerializeField] private BlackBoard m_blackBoard = new BlackBoard();
    #endregion

    #region Unity Methods
    void Start()
    {
        InitFSM();
    }

    void FixedUpdate()
    {
        m_crtState.OnUpdate(ref m_blackBoard, gameObject);
        TryChangeState();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Init Finit State Machine for the first time
    /// Need to be call before using this system
    /// </summary>
    private void InitFSM()
    {
        // Destroy current state if not null
        if (m_crtState)
        {
            m_crtState.OnExit(ref m_blackBoard, gameObject);
            Destroy(m_crtState);
        }

        if (m_initialState == null)
            return;

        // Instantiate new state (new scriptable object)
        m_crtState = Instantiate(m_initialState);

        if (!m_crtState)
            return;

        m_crtState.Init();
        m_crtState.OnEnter(ref m_blackBoard, gameObject);
    }

    /// <summary>
    /// Try to change the current state to the next possible state
    /// </summary>
    private void TryChangeState()
    {
        for (int i = 0; i < m_crtState.Transitions.Length; ++i)
        {
            if (m_crtState.Transitions[i] == null)
                continue;

            // If transition is valid, then we destroy the current state and enter into the new one
            if (m_crtState.Transitions[i].IsValid(m_blackBoard, transform.gameObject))
            {
                // End current state
                m_crtState.OnExit(ref m_blackBoard, gameObject);
                Destroy(m_crtState);
                
                // Start new State
                m_crtState = Instantiate(m_crtState.Transitions[i].NextState);
                if (!m_crtState)
                    return;
                m_crtState.Init();
                m_crtState.OnEnter(ref m_blackBoard, gameObject);
            }
        }
    }
    #endregion
}