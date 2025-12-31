using UnityEngine;

public class DialogueInputHandler : MonoBehaviour
{
    [SerializeField] private DialogueController controller;

    [Header("Advance 기본키(고정 UX)")]
    [SerializeField] private KeyCode[] advanceKeys = { KeyCode.Space, KeyCode.Return, KeyCode.KeypadEnter };

    [SerializeField] private bool allowMouseClick = true;
    [SerializeField] private bool allowAnyKeyAdvance = false; 

    void Awake()
    {
        if (controller == null) controller = GetComponent<DialogueController>();
    }

    void Update()
    {
        if (controller == null || !controller.IsDialogueRunning) return;

        if (IsAdvancePressed())
        {
            controller.AdvanceDialogue();
            return;
        }

        var im = InputManager.Instance;
        if (im == null) return;

        if (im.GetAction(ActionId.DialogueFastForward))
        {
            // FastForward 함수 정의
        }
        else
        {
            //
        }
    }

    bool IsAdvancePressed()
    {
        if (allowMouseClick && Input.GetMouseButtonDown(0)) return true;

        for (int i = 0; i < advanceKeys.Length; i++)
            if (Input.GetKeyDown(advanceKeys[i])) return true;

        if (allowAnyKeyAdvance && Input.anyKeyDown) return true;

        return false;
    }
}