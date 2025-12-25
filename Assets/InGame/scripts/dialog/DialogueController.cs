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

    // 대화 시작
    public bool BeginDialogue(DialogueSO dialogue, Action onComplete = null)
    {
        // 이미 실행 중이면 실패
        if (IsDialogueRunning)
        {
            Debug.LogWarning("DialogueController: 이미 대화가 실행 중입니다.");
            return false;
        }

        if (dialogue == null || dialogue.lines == null || dialogue.lines.Length == 0)
        {
            Debug.LogWarning("DialogueController: 유효하지 않은 DialogueSO");
            return false;
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

        return true;
    }

    // 다음 대사 처리
    public void AdvanceDialogue()
    {
        if (!IsDialogueRunning) return;

        if (IsTyping)
        {
            view?.CompleteTypingImmediately();
            return;
        }

        if (currentIndex + 1 < lines.Length)
            NextLine();
        else
            EndDialogueInternal();
    }

    // 강제로 대화 종료
    public void ForceEndDialogue()
    {
        if (!IsDialogueRunning) return;
        EndDialogueInternal();
    }

    // 다음 대사 처리
    private void NextLine()
    {
        currentIndex++;
        
        // 범위 체크
        if (currentIndex < 0 || currentIndex >= lines.Length)
        {
            Debug.LogWarning($"DialogueController: 인덱스 범위 초과 ({currentIndex}/{lines.Length})");
            EndDialogueInternal();
            return;
        }

        DialogueLine currentLine = lines[currentIndex];

        characterManager?.HandleLine(currentLine);
        view?.ShowLine(currentLine);
    }

    // 대화 종료
    private void EndDialogueInternal()
    {
        IsDialogueRunning = false;

        view?.HideAll();
        characterManager?.ClearCharacters();

        onComplete?.Invoke();
        onComplete = null;
    }
}
