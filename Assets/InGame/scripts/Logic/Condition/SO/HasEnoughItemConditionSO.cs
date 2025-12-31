using UnityEngine;


[CreateAssetMenu(
    menuName = "Condition/Item/Has Enough Item",
    fileName = "HasEnoughItemCondition"
)]
public class HasEnoughItemConditionSO : ItemConditionSO
{
    [SerializeField] private int requiredCount = 1;

    public override bool IsMet()
    {
        return GetItemCount() >= requiredCount;
    }
}
