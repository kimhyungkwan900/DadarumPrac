using UnityEngine;

// 특정 영역 입장 시 지도 UI를 표시하는 트리거
[RequireComponent(typeof(Collider2D))]
public class MapAreaTrigger : MonoBehaviour
{
    #region 컴포넌트
    [Header("References")]
    [SerializeField]
    private MapNode mapNode;

    [SerializeField]
    private MapUI mapUI;
    #endregion

    #region 필드
    private Collider2D triggerCollider;
    private bool hasEntered = false;
    #endregion

    #region 초기화
    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        if (mapUI == null)
            mapUI = FindObjectOfType<MapUI>();
    }
    #endregion

    #region Trigger 감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어만 처리
        if (!other.CompareTag("Player"))
            return;

        // 한 번만 실행
        if (hasEntered)
            return;

        hasEntered = true;

        // 현재 맵 노드 설정
        if (MoveService.Instance != null && mapNode != null)
        {
            MoveService.Instance.SetCurrentMapNode(mapNode);
        }

        // 지도 UI 표시
        if (mapUI != null && mapNode != null)
        {
            mapUI.OpenMap(mapNode);
        }
        else
        {
            Debug.LogWarning("[MapAreaTrigger] mapUI 또는 mapNode가 설정되지 않았습니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasEntered = false;
        }
    }
    #endregion
}

