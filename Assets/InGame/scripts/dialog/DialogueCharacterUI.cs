using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueCharacterUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;

    private CharacterProfileSO profile;

    private void Awake()
    {
        if (image == null)
            image = GetComponentInChildren<Image>(true);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (image == null)
            Debug.LogError($"{name}: 이미지 Reference 없음");
    }

    public void Initialize(CharacterProfileSO profile)
    {
        this.profile = profile;
    }

    public void SetExpression(ExpressionKey key, Sprite overrideSprite)
    {
        if (overrideSprite != null)
        {
            image.sprite = overrideSprite;
            return;
        }

        image.sprite = profile.GetExpression(key);
    }

    public void SetFocus(bool focused)
    {
        image.color = focused
            ? Color.white
            : new Color(0.9f, 0.9f, 0.9f, 1f);
    }

    #region 애니메이션 영역
    public void PlayAppear()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void PlayDisappear(Action onComplete)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        onComplete?.Invoke();
    }

    public void PlayShake()
    {
        // 필요 시 DOTween / LeanTween
    }
    #endregion
}
