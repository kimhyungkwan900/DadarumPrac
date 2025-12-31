using UnityEngine;

// 플레이어의 상호작용 처리
public class PlayerInteraction : MonoBehaviour
{
    #region 필드

    // 현재 상호작용 가능한 오브젝트
    private GameObject current;

    #endregion

    #region 업데이트

    // 입력 확인 및 상호작용 처리
    private void Update()
    {
        if (InputManager.Instance.GetAction(ActionId.Interact) &&
            InputManager.Instance.CurrentContext == InputContext.Player &&
            current != null)
        {
            TryInteract(current);
        }
    }

    #endregion

    #region 상호작용

    // 상호작용 시도
    private void TryInteract(GameObject target)
    {
        if (target.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(gameObject);
        }
    }

    #endregion

    #region Trigger 감지

    // Trigger 진입 시 상호작용 가능한 오브젝트로 설정
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            current = other.gameObject;
        }
    }

    // Trigger 탈출 시 상호작용 가능한 오브젝트 해제
    private void OnTriggerExit2D(Collider2D other)
    {
        if (current == other.gameObject)
        {
            current = null;
        }
    }

    #endregion
}
