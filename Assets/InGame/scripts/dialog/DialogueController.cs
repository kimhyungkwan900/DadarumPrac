using System;
using UnityEngine;

public class DialogueController : MonoBehaviour, IDialogueController
{
    [Header("컴포넌트")]
    [SerializeField] private DialogueView view;
    [SerializeField] private DialogueCharacterManager characterManager;
    [SerializeField] private ChoiceManager choiceManager;

    private DialogueSO currentDialogue;
    private DialogueLine[] lines;
    private int currentIndex = -1;

    public bool IsDialogueRunning { get; private set; }
    public bool IsTyping => view != null && view.IsTyping;
    private bool isWaitingForChoice;

    private Action onComplete;

    private void Awake()
    {
        if (view == null)
            view = GetComponentInChildren<DialogueView>(true);

        if (characterManager == null)
            characterManager = GetComponent<DialogueCharacterManager>();

        Debug.Assert(characterManager != null, "DialogueCharacterManager 필요.");

        if (choiceManager == null)
            choiceManager = GetComponent<ChoiceManager>();
        Debug.Assert(choiceManager != null, "ChoiceManager 필요.");

        view?.Initialize();
    }

    // 대화 시작
    public bool BeginDialogue(DialogueSO dialogue, Action onFinished = null)
    {
        if (dialogue == null || IsDialogueRunning)
            return false;

        currentDialogue = dialogue;
        lines = dialogue.lines;
        currentIndex = -1;
        onComplete = onFinished;

        IsDialogueRunning = true;

        view?.Initialize();

        if (dialogue.initialCharacters != null && characterManager != null)
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
        if (!IsDialogueRunning || isWaitingForChoice) return;

        if (IsTyping)
        {
            view?.CompleteTypingImmediately();
            return;
        }
        
        // 현재 대사 처리
        var line = lines[currentIndex];

        // 대사 종료 이벤트 호출
        InvokeLineEnd(line);

        if (line.choiceGroup != null){
            isWaitingForChoice = true;
            choiceManager?.ShowChoices(line.choiceGroup, OnChoiceSelected);
            return;
        }
        
        GoNextOrEnd();
    }

    #region 대화 처리

    // 선택지 선택 처리
    private void OnChoiceSelected(ChoiceData choice){
        isWaitingForChoice = false;

        GoNextOrEnd();
    }


    // 다음 대사 또는 대화 종료
    private void GoNextOrEnd()
    {
        if (currentIndex + 1 < lines.Length)
            NextLine();
        else
            EndDialogueInternal();
    }

    // 강제로 대화 종료
    public void ForceEndDialogue()
    {
        if (!IsDialogueRunning) return;

        if (currentIndex >= 0 && currentIndex < lines.Length)
        {
            InvokeLineEnd(lines[currentIndex]);
        }

        EndDialogueInternal();
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

        InvokeLineStart(currentLine);

        characterManager?.HandleLine(currentLine);
        view?.ShowLine(currentLine);
    }
    #endregion

    #region 이벤트 훅

    // 대사 시작 시 호출
    private void InvokeLineStart(DialogueLine line)
    {
        if (line.onLineStartEffects != null)
        {
            foreach (var effect in line.onLineStartEffects)
                effect?.Apply();
        }
    }

    // 대사 종료 시 호출
    private void InvokeLineEnd(DialogueLine line)
    {

        if (line.onLineEndEffects != null)
        {
            foreach (var effect in line.onLineEndEffects)
                effect?.Apply();
        }
    }
    #endregion
}
