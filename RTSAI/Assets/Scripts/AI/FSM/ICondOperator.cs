using UnityEngine;

[CreateAssetMenu(fileName = "ICondOperator", menuName = "FSM/Conditions/ICondOperator")]
public class ICondOperator : ICondition
{
    private enum OPERATOR
    {
        AND,
        OR
    }

    [SerializeField] private OPERATOR m_operator;
    [SerializeField] private ICondition[] m_conditions;

    public override bool IsValid(BlackBoard _blackBoard, GameObject _owner)
    {
        bool result = false;

        switch (m_operator)
        {
            case OPERATOR.AND:
                result = ANDCondition(_blackBoard, _owner);
                break;
            case OPERATOR.OR:
                result = ORCondition(_blackBoard, _owner);
                break;
            default:
                break;
        }
        return bIsInverted ? !result : result;
    }

    private bool ANDCondition(BlackBoard _blackBoard, GameObject _owner)
    {
        foreach (ICondition condition in m_conditions)
        {
            if (condition)
            {
                if (!condition.IsValid(_blackBoard, _owner))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool ORCondition(BlackBoard _blackBoard, GameObject _owner)
    {
        foreach (ICondition condition in m_conditions)
        {
            if (condition)
            {
                if (condition.IsValid(_blackBoard, _owner))
                {
                    return true;
                }
            }
        }
        return false;
    }
}