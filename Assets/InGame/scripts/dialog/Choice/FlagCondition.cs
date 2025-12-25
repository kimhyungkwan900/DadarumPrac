using UnityEngine;

public class FlagCondition : IChoiceCondition
{
    public string flagName;
    public bool flagValue = true;

    public bool isMet()
    {
        return SaveManager.I.GetFlag(flagName) == flagValue;
    }
}
