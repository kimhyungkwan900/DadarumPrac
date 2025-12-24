using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 이 스크립트는 UI 요소가 드롭될 수 있는 영역으로 작동하게 합니다.
public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    // 하이라이트 효과를 주기 위한 Image 컴포넌트.
    private Image image;
    // 드롭되는 아이템의 위치를 잡아주기 위한 RectTransform.
    private RectTransform rect;

    // 기본 색상과 하이라이트 색상을 인스펙터에서 설정할 수 있도록 public으로 선언합니다.
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        // 시작할 때 이미지의 색상을 기본 색상으로 설정합니다.
        if (image != null)
        {
            image.color = defaultColor;
        }
    }

    // 마우스 포인터가 이 UI 영역 안으로 들어왔을 때 호출됩니다. (IPointerEnterHandler)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 드래그 중인 오브젝트가 있고, 이 슬롯이 비어 있을 때만 하이라이트합니다.
        if (eventData.pointerDrag != null && GetComponentInChildren<DraggableUI>() == null && image != null)
        {
            // 드롭 가능한 상태임을 시각적으로 알리기 위해 하이라이트 색상으로 변경합니다.
            image.color = highlightColor;
        }
    }

    // 마우스 포인터가 이 UI 영역 밖으로 나갔을 때 호출됩니다. (IPointerExitHandler)
    public void OnPointerExit(PointerEventData eventData)
    {
        // 드래그 중인 상태에서 영역 밖으로 나갔을 때만 색상을 원래대로 복구합니다.
        // 슬롯이 찼는지 여부와 관계없이, 하이라이트 되었다면 다시 기본 색상으로 돌아가야 합니다.
        if (eventData.pointerDrag != null && image != null)
        {
            // 하이라이트 효과를 제거하고 기본 색상으로 되돌립니다.
            image.color = defaultColor;
        }
    }

    // 드래그 중인 오브젝트를 이 UI 위에서 놓았을 때(드롭) 호출됩니다. (IDropHandler)
    public void OnDrop(PointerEventData eventData)
    {
        // 드롭된 오브젝트가 있고, 이 슬롯이 비어 있을 때만 드롭을 허용합니다.
        if (eventData.pointerDrag != null && GetComponentInChildren<DraggableUI>() == null)
        {
            // 1. 드롭된 오브젝트(eventData.pointerDrag)의 부모를 이 DroppableUI 오브젝트로 설정합니다.
            //    이제 드롭된 아이템은 이 슬롯의 자식이 됩니다.
            eventData.pointerDrag.transform.SetParent(transform);

            // 2. 드롭된 아이템의 위치를 부모(이 오브젝트)의 중앙에 맞춥니다.
            //    position(월드 좌표) 대신 anchoredPosition(부모 기준 상대 좌표)을 0으로 설정하면
            //    피벗 위치에 상관없이 항상 정확하게 중앙에 배치되므로 더 안정적입니다.
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // 3. 드롭이 성공했으므로, 하이라이트됐던 색상을 다시 기본 색상으로 되돌립니다.
            if (image != null)
            {
                image.color = defaultColor;
            }
        }
    }
}
