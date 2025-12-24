using UnityEngine;
using UnityEngine.Events;

// 캐릭터 위치
public enum CharacterPosition { Left, Center, Right }

// 캐릭터 존재 상태(무대 상태)
public enum CharacterPresence
{
    Keep,            // 현재 상태 유지
    Appear,         // 무대에 등장
    Disappear     // 무대에서 퇴장
}

[System.Serializable]
public struct DialogueLine
{
    [Header("Speaker")]
    public CharacterProfileSO character;       // 캐릭터 프로필(없으면 나레이션)
    public string overrideName;                    // 이름 오버라이드

    [Header("Stage")]
    public CharacterPresence presence;       // 존재 상태(핵심)
    public CharacterPosition position;           // 등장/이동 위치(정책에 따라 사용)

    [Header("Visuals")]
    public ExpressionKey expressionKey;      // 표정 키
    public Sprite overrideSprite;                    // 표정 스프라이트 오버라이드
    public bool focus;                                   // 강조(알파X, 색/효과 등)
    public bool shake;                                  // 떨림(일시 이벤트)

    [Header("Text")]
    [TextArea(2, 4)]
    public string dialogue;

    // 이벤트 훅(선택)
    public UnityEvent onLineStart;
    public UnityEvent onLineEnd;
}
