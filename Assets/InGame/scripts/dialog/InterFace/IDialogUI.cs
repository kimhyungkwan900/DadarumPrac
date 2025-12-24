using UnityEngine;

public interface IDialogueUI
{
    void ShowLine(DialogueLine line);
    void ShowTypingEffect(string text, float speed, System.Action onComplete);
    void HideAll();
}
