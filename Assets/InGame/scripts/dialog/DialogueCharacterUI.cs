using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueCharacterUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;

    private CharacterProfileSO profile;
    public CharacterPosition Position { get; private set; } = CharacterPosition.Center;

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

    public void SetPosition(CharacterPosition position)
    {
        Position = position;
    }

    #region 애니메이션 관련 메서드

    public void PlayAppear()
    {
        // TODO: 애니메이션 추가
    }

    public void PlayDisappear()
    {
        // TODO: 애니메이션 추가
    }

    #endregion

    #region UI 관련

    public void Show(){
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    public void Hide(){
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
    }
    #endregion
}
