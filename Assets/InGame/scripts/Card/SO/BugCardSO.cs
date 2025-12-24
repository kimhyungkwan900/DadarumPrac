using UnityEngine;

[CreateAssetMenu(fileName = "NewBugCard", menuName ="Card/Bug") ]
public class BugCardSO : ScriptableObject
{
    [Header("비공개 ID값")]
    public int id;

    [Header("기본 정보")]
    public string cardName;
    public Sprite artwork;

    [Header("스탯")]
    public int attack;
    public int health;

    [Header("특수능력")]
    public CardAbilitySO specialAbility;
}
