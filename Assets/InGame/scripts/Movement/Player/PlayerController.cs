using UnityEngine;

[RequireComponent (typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        var input = InputManager.Instance != null ? InputManager.Instance.MoveInput : Vector2.zero;
        movement.Move(input);
    }
}
