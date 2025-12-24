using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Set")]
public class CardSetSO : ScriptableObject
{
    [Header("표시용 정보")]
    public string setId;          // 내부용 ID
    public string displayName;    // UI에 보여줄 이름
    public Sprite icon;           // 선택 화면 등에서 보여줄 아이콘

    [Header("이 세트에 포함된 카드들")]
    public BugCardSO[] cards;     // 여기 안에 3장(or N장) 넣어두기
}
