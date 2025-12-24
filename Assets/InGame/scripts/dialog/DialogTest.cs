using System.Collections;
using TMPro;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueSO dialogueSO;

    private void Start()
    {
        if (dialogueController == null || dialogueSO == null)
        {
            Debug.LogWarning("DialogTest: 컨트롤러 혹은 SO 파일 미할당.");
            return;
        }

        dialogueController.BeginDialogue(dialogueSO, OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        Debug.Log("DialogTest: 다이얼로그 종료");
    }
}