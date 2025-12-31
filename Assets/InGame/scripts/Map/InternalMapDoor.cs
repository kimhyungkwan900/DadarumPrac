using UnityEngine;

// 내부 이동용 문 컴포넌트 (Interaction을 통해 이동)
[RequireComponent(typeof(Collider2D))]
public class InternalMapDoor : MonoBehaviour, IInteractable
{
    #region 컴포넌트
    [Header("References")]
    [SerializeField]
    private MapDestination destination;

    [SerializeField]
    private string interactionPrompt = "이동";
    #endregion

    #region 필드
    private Collider2D triggerCollider;
    #endregion

    #region 초기화
    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }
    #endregion

    #region 상호작용
    // 상호작용 처리
    public void Interact(GameObject interactor)
    {
        if (destination == null)
        {
            Debug.LogWarning("[InternalMapDoor] destination이 설정되지 않았습니다.");
            return;
        }

        // 내부 이동만 허용
        if (destination.moveRange != MapMoveRange.Internal)
        {
            Debug.LogWarning("[InternalMapDoor] 내부 이동이 아닙니다. MapMoveRange를 Internal로 설정하세요.");
            return;
        }

        if (MoveService.Instance == null)
        {
            Debug.LogWarning("[InternalMapDoor] MoveService가 없습니다.");
            return;
        }

        // 이동 실행 (에너지 소모 없음)
        MoveService.Instance.TryMoveTo(destination);
    }

    // 상호작용 프롬프트 반환 (필요시 UI에 표시)
    public string GetInteractionPrompt() => interactionPrompt;
    #endregion
}

