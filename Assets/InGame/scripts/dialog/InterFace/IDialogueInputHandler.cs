using UnityEngine;

public class IDialogueInputHandler : MonoBehaviour
{
    [SerializeField] private DialogueController controller;

    [Header("Keys")]
    [SerializeField] private KeyCode advanceKey = KeyCode.Space;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<DialogueController>();
    }

    private void Update()
    {
        if (controller == null || !controller.IsDialogueRunning)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(advanceKey))
        {
            controller.AdvanceDialogue();
        }
    }
}
