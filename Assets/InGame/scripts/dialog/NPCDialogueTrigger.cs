using UnityEngine;
[System.Serializable]
public class DialogueConditionEntry
{
    public DialogueSO dialogue;
    public DialogueCondition condition;
}
public enum DialogueCondition
{
    Default,
    FirstMeet,
}

public class NPCDialogueTrigger : MonoBehaviour, IDialogueTrigger
{
    [Header("Dialogue Conditions")]
    [SerializeField] private DialogueConditionEntry[] dialogues;

    [Header("Dialogue System")]
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private GameObject dialogueSystemRoot;

    private bool isRunning;

    public void TriggerDialogue()
    {
        if (isRunning) return;

        DialogueSO selected = ResolveDialogue();
        if (selected == null)
        {
            Debug.LogWarning($"{name}: 조건에 맞는 DialogueSO가 없습니다.");
            return;
        }

        isRunning = true;

        // DialogueSystem 활성화
        dialogueSystemRoot.SetActive(true);

        // Input Context 전환
        InputManager.Instance.PushContext(InputContext.Dialogue);

        // Dialogue 시작
        dialogueController.BeginDialogue(selected, OnDialogueFinished);
    }

    private DialogueSO ResolveDialogue()
    {
        // 가장 먼저 조건을 만족하는 Dialogue를 선택
        foreach (var entry in dialogues)
        {
            if (CheckCondition(entry.condition))
                return entry.dialogue;
        }

        return null;
    }

    private bool CheckCondition(DialogueCondition condition)
    {
        switch (condition)
        {
            case DialogueCondition.FirstMeet:
                return true;

            case DialogueCondition.Default:
                return true;
        }
        return false;
    }

    private void OnDialogueFinished()
    {
        dialogueSystemRoot.SetActive(false);
        InputManager.Instance.PopContext();
        isRunning = false;
    }
}
