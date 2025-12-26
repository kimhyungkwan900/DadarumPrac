using UnityEngine;

public enum CompareType
{
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual,
    Equal,
    NotEqual
}

[CreateAssetMenu(menuName = "Condition/Variable Compare")]
public class VariableCompareConditionSO : ConditionSO
{
    public string variableKey;
    public CompareType compareType;
    public int value;

    public override bool IsMet()
    {
        int current = SaveManager.I.GetVar(variableKey);

        return compareType switch
        {
            CompareType.Greater => current > value,
            CompareType.GreaterOrEqual => current >= value,
            CompareType.Less => current < value,
            CompareType.LessOrEqual => current <= value,
            CompareType.Equal => current == value,
            CompareType.NotEqual => current != value,
            _ => false
        };
    }
}
