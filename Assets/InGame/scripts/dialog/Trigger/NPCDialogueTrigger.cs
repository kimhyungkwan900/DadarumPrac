using System.Collections.Generic;
using UnityEngine;

// 대화 조건 엔트리
[System.Serializable]
public class DialogueConditionEntry
{
    public DialogueSO dialogue;
    [Tooltip("조건이 비어 있으면 항상 통과 (Default 대화)")]
    public List<ConditionSO> conditions;
}

// NPC 대화 트리거 컴포넌트
public class NPCDialogueTrigger : MonoBehaviour, IDialogueTrigger, IInteractable
{
    [Header("Dialogue Conditions")]
    [SerializeField, Tooltip("위에서 아래 순서로 평가됨")]
    private DialogueConditionEntry[] dialogues;

    [Header("Dialogue System")]
    [SerializeField, Tooltip("대화 컨트롤러 (자동 찾기 가능)")]
    private DialogueController dialogueController;
    
    [SerializeField, Tooltip("대화 시스템 루트 오브젝트")]
    private GameObject dialogueSystemRoot;

    private bool isRunning;

    private void Awake()
    {
        if (dialogueController == null)
            dialogueController = FindObjectOfType<DialogueController>(true);

        if (dialogueSystemRoot == null && dialogueController != null)
        {
            Transform parent = dialogueController.transform.parent;
            if (parent != null)
                dialogueSystemRoot = parent.gameObject;
        }

        if (dialogueSystemRoot != null)
            dialogueSystemRoot.SetActive(false);
    }

    // 상호작용 시 대화 시작
    public void Interact(GameObject interactor)
    {
        TriggerDialogue();
    }

    // 대화 트리거
    public void TriggerDialogue()
    {
        if (isRunning)
            return;

        if (dialogueController == null || dialogueSystemRoot == null)
        {
            Debug.LogError($"{name}: DialogueSystem 참조 누락", this);
            return;
        }

        DialogueSO selected = ResolveDialogue();
        if (selected == null)
        {
            Debug.LogWarning($"{name}: 조건에 맞는 DialogueSO 없음", this);
            return;
        }

        StartDialogue(selected);
    }

    // 대화 시작 처리
    private void StartDialogue(DialogueSO dialogue)
    {
        isRunning = true;
        dialogueSystemRoot.SetActive(true);

        if (InputManager.Instance != null)
            InputManager.Instance.PushContext(InputContext.Dialogue);

        // BeginDialogue 성공 여부 확인
        bool started = dialogueController.BeginDialogue(dialogue, OnDialogueFinished);
        if (!started)
        {
            RollbackDialogue();
        }
    }

    private void OnDialogueFinished()
    {
        RollbackDialogue();
    }

    private void RollbackDialogue()
    {
        if (dialogueSystemRoot != null)
            dialogueSystemRoot.SetActive(false);

        if (InputManager.Instance != null)
            InputManager.Instance.PopContext();

        isRunning = false;
    }

    // 조건에 맞는 대화 해결
    private DialogueSO ResolveDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
            return null;

        foreach (var entry in dialogues)
        {
            if (entry.dialogue == null)
                continue;

            if (AreConditionsMet(entry.conditions))
                return entry.dialogue;
        }

        return null;
    }

    // 강제로 대화 종료
    public void ForceEndDialogue()
    {
        if (!isRunning)
            return;

        if (dialogueController != null)
            dialogueController.ForceEndDialogue();
        else
            RollbackDialogue();
    }

    private bool AreConditionsMet(List<ConditionSO> conditions)
    {
        // Default 대화
        if (conditions == null || conditions.Count == 0)
            return true;

        foreach (var cond in conditions)
        {
            if (cond == null)
            {
                Debug.LogWarning($"{name}: Null Condition 발견", this);
                continue;
            }

            if (!cond.IsMet())
                return false;
        }

        return true;
    }
    private void OnDestroy()
    {
        if (isRunning)
            ForceEndDialogue();
    }
}