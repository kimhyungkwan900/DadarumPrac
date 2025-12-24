using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement movement;

    static readonly int IsMoving = Animator.StringToHash("IsMoving");
    static readonly int MoveX= Animator.StringToHash("MoveX");
    static readonly int MoveY = Animator.StringToHash("MoveY");

    Vector2 lastDir = Vector2.zero;

    private void OnEnable()
    {
        movement.OnMove += HandleMove;
        movement.OnMoveStop += HandleStop;
    }
    void OnDisable()
    {
        movement.OnMove -= HandleMove;
        movement.OnMoveStop -= HandleStop;
    }

    void HandleMove(Vector2 dir)
    {
        lastDir = dir;
        anim.SetBool(IsMoving, true);
        anim.SetFloat(MoveX, dir.x);
        anim.SetFloat(MoveY, dir.y);
    }

    void HandleStop()
    {
        anim.SetBool(IsMoving, false);

        anim.SetFloat(MoveX, lastDir.x);
        anim.SetFloat(MoveY, lastDir.y);
    }
}
