using System;

public interface IDialogueController
{
    void BeginDialogue(DialogueSO dialogue, Action onComplete = null);
    void AdvanceDialogue();
    bool IsDialogueRunning { get; }
}
