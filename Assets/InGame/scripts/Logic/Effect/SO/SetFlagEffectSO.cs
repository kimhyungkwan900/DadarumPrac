using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Apply();
}

[CreateAssetMenu(menuName = "Effect/Variable/Set Flag")]
public class SetFlagEffectSO : EffectSO
{
    public string flagName;
    public bool flagValue = true;

    public override void Apply()
    {
        SaveManager.I.SetFlag(flagName, flagValue);
    }
}