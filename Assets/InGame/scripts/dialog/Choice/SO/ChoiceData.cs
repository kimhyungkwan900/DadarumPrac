using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceData
{
    [SerializeField]
    public string text;
    [SerializeField]
    public List<ConditionSO> conditions;
    [SerializeField] 
    public List<EffectSO> effects;
}

[CreateAssetMenu(menuName = "Choice/Choice Group")]
public class ChoiceGroupSO : ScriptableObject
{
    public string id;
    public List<ChoiceData> choices = new();
}

