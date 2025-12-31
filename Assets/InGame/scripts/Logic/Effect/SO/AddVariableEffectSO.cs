using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Variable/Add Variable")]
public class AddVariableEffectSO : EffectSO
{
    public string variableKey;
    public int delta;

    public override void Apply()
    {
        SaveManager.I.AddVar(variableKey, delta);
    }
}

