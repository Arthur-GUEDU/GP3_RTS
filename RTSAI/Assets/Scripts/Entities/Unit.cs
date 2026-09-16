using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class Unit : BaseEntity
{
    [SerializeField]
    UnitDataScriptable UnitData = null;

    Transform BulletSlot;
    float LastActionDate = 0f;
    public Target Target = new Target();
    [HideInInspector] public NavMeshAgent NavMeshAgent;
    [SerializeField] private SphereCollider m_detectionCollider = null;
    [HideInInspector] private bool bIsAgressive = false;
    [HideInInspector] private bool bIsTriggered = false;
    [SerializeField] private List<Unit> m_triggerUnits = new List<Unit>();

    public bool IsAgressive { get { return bIsAgressive; } }
    public bool IsTriggered { get { return bIsTriggered; } }
    public List<Unit> TriggerUnits { get { return m_triggerUnits; } }

    List<GoapAction> currentPlan;

    Squad m_squad = null;

    public Squad Squad { get { return m_squad; } }

    public UnitDataScriptable GetUnitData { get { return UnitData; } }
    public int Cost { get { return UnitData.Cost; } }
    public int GetTypeId { get { return UnitData.TypeId; } }
    override public void Init(ETeam _team)
    {
        if (IsInitialized)
            return;

        base.Init(_team);

        HP = UnitData.MaxHP;
        OnDeadEvent += Unit_OnDead;
    }
    void Unit_OnDead()
    {
        if (IsCapturing())
            StopCapture();

        if (GetUnitData.DeathFXPrefab)
        {
            GameObject fx = Instantiate(GetUnitData.DeathFXPrefab, transform);
            fx.transform.parent = null;
        }

        Destroy(gameObject);
    }

    public void SetSquad(Squad squad)
    {
        if (m_squad != null)
            m_squad.RemoveUnit(this);
        m_squad = squad;
    }

    public void SetAgressive(bool _isAgressive)
    {
        bIsAgressive = _isAgressive;
        if (m_detectionCollider)
            m_detectionCollider.enabled = bIsAgressive;
    }

    #region MonoBehaviour methods
    override protected void Awake()
    {
        base.Awake();

        NavMeshAgent = GetComponent<NavMeshAgent>();
        BulletSlot = transform.Find("BulletSlot");

        // fill NavMeshAgent parameters
        NavMeshAgent.speed = GetUnitData.Speed;
        NavMeshAgent.angularSpeed = GetUnitData.AngularSpeed;
        NavMeshAgent.acceleration = GetUnitData.Acceleration;
    }
    override protected void Start()
    {
        // Needed for non factory spawned units (debug)
        if (!IsInitialized)
            Init(Team);

        if (m_detectionCollider)
        {
            m_detectionCollider.isTrigger = true;
            m_detectionCollider.radius = GetUnitData.DetectionRange;
            m_detectionCollider.enabled = false;
        }
        else
        {
            UnityEngine.Debug.LogError("Error: Failed to get sphere collider component!");
        }

        SetAgressive(true);

        base.Start();
    }
    override protected void Update()
    {
        // Attack / repair task debug test $$$ to be removed for AI implementation
        if (Target.EntityTarget != null)
        {
            if (Target.EntityTarget.GetTeam() != GetTeam())
                ComputeAttack();
            else
                ComputeRepairing();
        }

        if (bIsTriggered)
        {
            AttackEnemyBehaviour();
        }
    }

    override protected void OnTriggerEnter(Collider _other)
    {
        base.OnTriggerEnter(_other);
        if (!bIsAgressive)
            return;

        Unit unit;
        if (_other.TryGetComponent(out unit))
        {
            if (unit.Team != Team && !m_triggerUnits.Contains(unit))
            {
                bIsTriggered = true;
                m_triggerUnits.Add(unit);
                if (m_triggerUnits.Count == 1)
                    m_squad?.GetCurrentAction().PauseAction(this);
            }
        }
    }

    override protected void OnTriggerExit(Collider _other)
    {
        base.OnTriggerEnter(_other);
        if (!bIsAgressive)
            return;


        Unit unit;
        if (_other.TryGetComponent(out unit))
        {
            if (unit.Team != Team)
            {
                m_triggerUnits.Remove(unit);
                if (m_triggerUnits.Count == 0)
                {
                    bIsTriggered = false;
                    m_squad.GetCurrentAction().UnpauseAction(this);
                }
            }
        }
    }
    #endregion

    private void AttackEnemyBehaviour()
    {
        if (bIsTriggered == false)
            return;

        if (m_triggerUnits.Count == 0)
        {
            bIsTriggered = false;
            m_squad.GetCurrentAction()?.UnpauseAction(this);
            return;
        }

        Unit nearestUnit = null;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < m_triggerUnits.Count; ++i)
        {
            if (m_triggerUnits[i] == null)
                continue;

            float distance = Vector3.Distance(m_triggerUnits[i].transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestUnit = m_triggerUnits[i];
                nearestDistance = distance;
            }
        }

        if (nearestUnit != null)
        {
            if (CanAttack(nearestUnit))
                SetAttackTarget(nearestUnit);
            else
                MoveToEnemyBehaviour(nearestUnit);
        }
    }

    private void MoveToEnemyBehaviour(BaseEntity _entity)
    {
        if (_entity == null)
            return;

        if (Vector3.Distance(_entity.transform.position, transform.position) > GetUnitData.AttackDistanceMax)
        {
            Vector3 dir = (transform.position - _entity.transform.position).normalized * (GetUnitData.AttackDistanceMax * 0.9f);
            SetTargetPos(_entity.transform.position + dir);
        }
    }

    #region IRepairable
    override public bool NeedsRepairing()
    {
        return HP < GetUnitData.MaxHP;
    }
    override public void Repair(int amount)
    {
        HP = Mathf.Min(HP + amount, GetUnitData.MaxHP);
        base.Repair(amount);
    }
    override public void FullRepair()
    {
        Repair(GetUnitData.MaxHP);
    }
    #endregion

    #region Tasks methods : Moving, Capturing, Targeting, Attacking, Repairing ...

    // Used in MoveTo Action
    public void SetTargetPos(Vector3 pos)
    {
        if (NavMeshAgent)
        {
            NavMeshAgent.SetDestination(pos);
            NavMeshAgent.isStopped = false;
        }
    }

    // Targetting Task - attack
    public void SetAttackTarget(BaseEntity target)
    {
        if (CanAttack(target) == false)
            return;

        if (Target.CaptureTarget != null)
            StopCapture();

        if (CanAttack(target) && target.GetTeam() != GetTeam())
            StartAttacking(target);
    }

    // Targetting Task - capture
    public void SetCaptureTarget(TargetBuilding target)
    {
        if (CanCapture(target) == false)
            return;

        if (Target.EntityTarget != null)
            Target.EntityTarget = null;

        if (IsCapturing())
            StopCapture();

        if (target.GetTeam() != GetTeam())
            StartCapture(target);
    }

    // Targetting Task - repairing
    public void SetRepairTarget(BaseEntity entity)
    {
        if (CanRepair(entity) == false)
            return;

        if (Target.CaptureTarget != null)
            StopCapture();

        if (entity.GetTeam() == GetTeam())
            StartRepairing(entity);
    }

    public bool CanAttack(BaseEntity target)
    {
        if (target == null)
            return false;

        // distance check
        if ((target.transform.position - transform.position).sqrMagnitude > GetUnitData.AttackDistanceMax * GetUnitData.AttackDistanceMax)
            return false;

        return true;
    }

    // Attack Task
    public void StartAttacking(BaseEntity target)
    {
        Target.EntityTarget = target;
    }

    public void ComputeAttack()
    {
        if (CanAttack(Target.EntityTarget) == false)
            return;

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;

        transform.LookAt(Target.EntityTarget.transform);
        // only keep Y axis
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = 0f;
        eulerRotation.z = 0f;
        transform.eulerAngles = eulerRotation;

        if ((Time.time - LastActionDate) > UnitData.AttackFrequency)
        {
            LastActionDate = Time.time;
            // visual only
            if (UnitData.BulletPrefab)
            {
                GameObject newBullet = Instantiate(UnitData.BulletPrefab, BulletSlot);
                newBullet.transform.parent = null;
                newBullet.GetComponent<Bullet>().ShootToward(Target.EntityTarget.transform.position - transform.position, this);
            }
            // apply damages
            int damages = Mathf.FloorToInt(UnitData.DPS * UnitData.AttackFrequency);
            Target.EntityTarget.AddDamage(damages);
        }
    }

    public bool CanCapture(TargetBuilding target)
    {
        if (target == null)
            return false;

        // distance check
        if ((target.transform.position - transform.position).sqrMagnitude > GetUnitData.CaptureDistanceMax * GetUnitData.CaptureDistanceMax)
            return false;

        return true;
    }

    // Used in Capture Action
    public void StartCapture(TargetBuilding target)
    {
        if (CanCapture(target) == false)
            return;

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;

        Target.CaptureTarget = target;
        Target.CaptureTarget.StartCapture(this);
    }

    public void StopCapture()
    {
        if (Target.CaptureTarget == null)
            return;

        Target.CaptureTarget.StopCapture(this);
        Target.CaptureTarget = null;
    }

    public bool IsCapturing()
    {
        return Target.CaptureTarget != null;
    }

    // Repairing Task
    public bool CanRepair(BaseEntity target)
    {
        if (GetUnitData.CanRepair == false || target == null)
            return false;

        // distance check
        if ((target.transform.position - transform.position).sqrMagnitude > GetUnitData.RepairDistanceMax * GetUnitData.RepairDistanceMax)
            return false;

        return true;
    }
    public void StartRepairing(BaseEntity entity)
    {
        if (GetUnitData.CanRepair)
        {
            Target.EntityTarget = entity;
        }
    }
    public void ComputeRepairing()
    {
        if (CanRepair(Target.EntityTarget) == false)
            return;

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;

        transform.LookAt(Target.EntityTarget.transform);
        // only keep Y axis
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = 0f;
        eulerRotation.z = 0f;
        transform.eulerAngles = eulerRotation;

        if ((Time.time - LastActionDate) > UnitData.RepairFrequency)
        {
            LastActionDate = Time.time;

            // apply reparing
            int amount = Mathf.FloorToInt(UnitData.RPS * UnitData.RepairFrequency);
            Target.EntityTarget.Repair(amount);
        }
    }
    #endregion
}