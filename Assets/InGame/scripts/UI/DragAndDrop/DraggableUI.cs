using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    [Header("이동 연출 옵션")]
    [SerializeField] private bool useReturnTween = true;
    [SerializeField] private float returnDuration = 0.2f;
    [SerializeField] private Ease returnEase = Ease.OutQuad;

    private Tween currentTween;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>().rootCanvas;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        previousParent = transform.parent;

        transform.SetParent(canvas.transform, worldPositionStays: true);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                cam,
                out Vector2 localPos))
        {
            rect.anchoredPosition = localPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvas.transform)
        {
            transform.SetParent(previousParent, worldPositionStays: true);
            transform.localScale = Vector3.one;

            if (useReturnTween)
            {
                currentTween = rect
                    .DOAnchorPos(Vector2.zero, returnDuration)
                    .SetEase(returnEase);
            }
            else
            {
                rect.anchoredPosition = Vector2.zero;
            }
        }
    }

    private void OnDestroy()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}