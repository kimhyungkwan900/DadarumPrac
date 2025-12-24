using System;
using UnityEngine;

public class DialogueController : MonoBehaviour, IDialogueController
{
    [Header("컴포넌트")]
    [SerializeField] private DialogueView view;
    [SerializeField] private DialogueCharacterManager characterManager;

    private DialogueLine[] lines;
    private int currentIndex = -1;
    private Action onComplete;

    public bool IsDialogueRunning { get; private set; }
    public bool IsTyping => view != null && view.IsTyping;

    private void Awake()
    {
        if (view == null)
            view = GetComponentInChildren<DialogueView>(true);

        if (characterManager == null)
            characterManager = GetComponent<DialogueCharacterManager>();

        Debug.Assert(characterManager != null, "DialogueCharacterManager 필요.");

        view?.Initialize();
    }

    public void BeginDialogue(DialogueSO dialogue, Action onComplete = null)
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Length == 0)
        {
            Debug.LogWarning("DialogueController: 유효하지 않은 DialogueSO");
            return;
        }

        this.lines = dialogue.lines;
        this.onComplete = onComplete;

        IsDialogueRunning = true;
        currentIndex = -1;

        view?.Initialize();
        characterManager?.ClearCharacters();

        // 대화 시작 시 기본 캐릭터 배치
        if (dialogue.initialCharacters != null)
        {
            foreach (var init in dialogue.initialCharacters)
            {
                if (init.character == null) continue;
                characterManager.SpawnInitial(init.character, init.position, init.expressionKey);
            }
        }

        if (dialogue.autoStart)
            NextLine();
    }

    public void AdvanceDialogue()
    {
        if (!IsDialogueRunning) return;

        if (IsTyping)
        {
            view.CompleteTypingImmediately();
            return;
        }

        if (currentIndex + 1 < lines.Length)
            NextLine();
        else
            EndDialogue();
    }

    private void NextLine()
    {
        currentIndex++;
        DialogueLine currentLine = lines[currentIndex];

        characterManager?.HandleLine(currentLine);
        view?.ShowLine(currentLine);
    }

    private void EndDialogue()
    {
        IsDialogueRunning = false;
        view?.HideAll();
        characterManager?.ClearCharacters();

        onComplete?.Invoke();
        onComplete = null;
    }
}
