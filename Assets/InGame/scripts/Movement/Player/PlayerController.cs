using UnityEngine;

[RequireComponent (typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        var input = InputManager.Instance != null ? InputManager.Instance.MoveInput : Vector2.zero;
        movement.Move(input);
    }
}
