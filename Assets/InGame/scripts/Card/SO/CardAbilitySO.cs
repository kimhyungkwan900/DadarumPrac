using UnityEngine;

public abstract class CardAbilitySO : ScriptableObject
{
    public Sprite icon;
    public string abilityName;
    public string description;

    public abstract void Activate(GameObject owner, GameObject target);
}