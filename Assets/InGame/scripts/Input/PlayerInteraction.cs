using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IDialogueTrigger current;

    void Update()
    {
        if (InputManager.Instance.GetAction(ActionId.Interact) &&
            InputManager.Instance.CurrentContext == InputContext.Player &&
            current != null)
        {
            current.TriggerDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDialogueTrigger>(out var trigger))
            current = trigger;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IDialogueTrigger>(out var trigger) && trigger == current)
            current = null;
    }
}
