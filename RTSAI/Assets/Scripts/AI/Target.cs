using UnityEngine;

public class Target
{
    #region Variables
    public Vector3 PositionTarget = Vector3.zero;  // for MoveTo
    public BaseEntity EntityTarget = null;         // for Attack, Defend and Repair
    public TargetBuilding CaptureTarget = null;    // for Capture

    #endregion

    #region Constructors
    public Target()
    {

    }

    public Target(Vector3 _positionTarget)
    {
        PositionTarget = _positionTarget;
    }

    public Target(BaseEntity _entityTarget)
    {
        EntityTarget = _entityTarget;
        PositionTarget = EntityTarget.transform.position;
    }

    public Target(TargetBuilding _captureTarget)
    {
        CaptureTarget = _captureTarget;
        PositionTarget = CaptureTarget.transform.position;
    }

    #endregion

    #region Methods


    #endregion
}