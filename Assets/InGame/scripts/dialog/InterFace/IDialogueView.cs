using UnityEngine;

public interface IDialogueView
{
    void Initialize();
    void ShowLine(DialogueLine line);
    void CompleteTypingImmediately();
    void HideAll();
    bool IsTyping { get; }
}
