using UnityEngine;

public abstract class ConditionSO : ScriptableObject
{
    public abstract bool IsMet();
}

[CreateAssetMenu(menuName = "Condition/Flag")]
public class FlagConditionSO : ConditionSO
{
    public string flagName;
    public bool flagValue = true;

    public override bool IsMet()
    {
        return SaveManager.I.GetFlag(flagName) == flagValue;
    }
}