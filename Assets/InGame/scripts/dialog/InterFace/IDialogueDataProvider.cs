using UnityEngine;

public interface IDialogueDataProvider
{
    int LineCount { get; }
    DialogueLine GetLine(int index);
}
