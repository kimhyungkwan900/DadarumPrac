using UnityEngine;
public abstract class ItemConditionSO : ConditionSO
{
    [Header("Item")]
    [SerializeField] protected string itemKey;

    protected int GetItemCount()
    {
        return SaveManager.I.GetItemCount(itemKey);
    }
}

[CreateAssetMenu(
    menuName = "Condition/Item/Has Item",
    fileName = "HasItemCondition"
)]
public class HasItemConditionSO : ItemConditionSO
{
    public override bool IsMet()
    {
        return GetItemCount() > 0;
    }
}
