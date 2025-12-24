using UnityEngine;
public enum ExpressionKey
{
    Idle,
    Happy,
    Angry,
    Sad,
    Surprised
}

[CreateAssetMenu(menuName = "Character/Profile")]
public class CharacterProfileSO : ScriptableObject
{
    public string characterName;
    [Header("다이얼로그 유아이용 프리팹")]
    public GameObject prefab;

    [Header("기본 감정 스프라이트")]
    public ExpressionSprite[] portraits;

    public Sprite GetExpression(ExpressionKey key)
    {
        foreach (var p in portraits)
        {
            if (p.key == key && p.sprite != null)
                return p.sprite;
        }

        return portraits.Length > 0 ? portraits[0].sprite : null;
    }
}

[System.Serializable]
public struct ExpressionSprite
{
    public ExpressionKey key;
    public Sprite sprite;
}