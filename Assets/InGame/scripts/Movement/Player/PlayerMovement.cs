using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnMoveStart;
    public event Action OnMoveStop;

    [SerializeField] private float moveSpeed = 3.5f;

    private Rigidbody2D rb;
    private Vector2 desiredInput;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 input)
    {
        desiredInput = input;

        if (desiredInput != Vector2.zero)
        {
            OnMove?.Invoke(desiredInput);

            if (!isMoving)
            {
                isMoving = true;
                OnMoveStart?.Invoke();
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                OnMoveStop?.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        if (desiredInput == Vector2.zero) return;

        Vector2 nextPos = rb.position + desiredInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }
}
