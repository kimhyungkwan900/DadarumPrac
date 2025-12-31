using UnityEngine;

[CreateAssetMenu(
    fileName = "DialogueSO",
    menuName = "Dialogue/Dialogue Script")]
public class DialogueSO : ScriptableObject
{
    [Header("기본설정")]
    public bool autoStart = true;

    [Header("시작 캐릭터")]
    public InitialDialogueCharacter[] initialCharacters;

    [Header("대사")]
    public DialogueLine[] lines;

    [TextArea(2, 4)]
    public string description;
}

[System.Serializable]
public struct InitialDialogueCharacter
{
    public CharacterProfileSO character;
    public CharacterPosition position;
    public ExpressionKey expressionKey;
}

