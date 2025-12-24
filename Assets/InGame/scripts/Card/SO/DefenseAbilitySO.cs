using UnityEngine;

[CreateAssetMenu(fileName = "DefenseAbility", menuName = "Card/Ability/FlatDamageReduction")]
public class DefenseAbilitySO : CardAbilitySO
{
    public int flatReduction = 2; // 방어력 수치

    public override void Activate(GameObject owner, GameObject target)
    {
        // 이 능력은 전투 중 passively 작동하므로, Activate는 사용 안 할 수 있음
    }

    public int ReduceDamage(int incomingDamage)
    {
        return Mathf.Max(0, incomingDamage - flatReduction);
    }
}