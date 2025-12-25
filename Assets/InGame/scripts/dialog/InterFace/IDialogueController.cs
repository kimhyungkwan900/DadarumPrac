using System;

public interface IDialogueController
{
    // 대화를 시작합니다.
    // dialogue: 시작할 대화 스크립트
    // onComplete: 대화 종료 시 호출될 콜백
    // 대화 시작 성공 여부 (true: 성공, false: 실패)
    bool BeginDialogue(DialogueSO dialogue, Action onComplete = null);
    
    // 대화를 다음으로 진행합니다.
    void AdvanceDialogue();
    
    // 현재 대화가 실행 중인지 여부
    bool IsDialogueRunning { get; }
}
