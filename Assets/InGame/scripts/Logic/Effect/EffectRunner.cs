using System.Collections.Generic;
using UnityEngine;

public class EffectRunner : MonoBehaviour
{
    [SerializeField]
    private List<EffectSO> effects;

    public void Run()
    {
        if (effects == null) return;

        foreach (var effect in effects)
        {
            effect?.Apply();
        }
    }
}

