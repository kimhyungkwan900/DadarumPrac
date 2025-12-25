using UnityEngine;

public class SetFlagEffect : IChoiceEffect
{
    public string flagName;
    public bool flagValue = true;

    public void Apply()
    {
        SaveManager.I.SetFlag(flagName, flagValue);
    }
}
