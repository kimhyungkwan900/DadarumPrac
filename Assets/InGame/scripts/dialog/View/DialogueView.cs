using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour, IDialogueView
{
    #region 컴포넌트

    [Header("Dialogue UI설정")]
    [SerializeField] private Image dialogFrame;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private GameObject arrow;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    #endregion

    #region 필드

    public bool IsTyping { get; private set; }

    private Coroutine typingRoutine;
    private string fullTextCache = "";

    #endregion

    #region 초기화

    public void Initialize()
    {
        IsTyping = false;

        bodyText.text = "";
        nameLabel.text = "";
        arrow.SetActive(false);
        
        // 대화 프레임 활성화
        if (dialogFrame != null)
            dialogFrame.gameObject.SetActive(true);
    }

    #endregion

    #region 대사 표시

    public void ShowLine(DialogueLine line)
    {
        if (line.character == null)
        {
            Debug.LogWarning("DialogueView: DialogueLine의 캐릭터가 Null 입니다.");
            // 이름 없는 나레이션 등을 위해 이름만 비워둘 수 있음
            nameLabel.text = "";
        }
        else
        {
            // 이름 설정
            nameLabel.text =
                !string.IsNullOrEmpty(line.overrideName)
                    ? line.overrideName
                    : line.character.characterName;
        }

        // 대사 타이핑 시작
        StartTyping(line.dialogue);
    }

    #endregion

    #region 타이핑 효과

    private void StartTyping(string fullText)
    {
        fullTextCache = fullText ?? "";

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypingRoutine(fullTextCache));
    }

    private IEnumerator TypingRoutine(string fullText)
    {
        IsTyping = true;
        arrow.SetActive(false);
        bodyText.text = "";

        int index = 0;
        while (index < fullText.Length)
        {
            bodyText.text = fullText.Substring(0, index + 1);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
        arrow.SetActive(true);
    }

    #endregion

    #region 완료 및 숨기기
    public void CompleteTypingImmediately()
    {
        if (!IsTyping) return;

        IsTyping = false;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        bodyText.text = fullTextCache;
        arrow.SetActive(true);
    }

    public void HideAll()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        IsTyping = false;

        bodyText.text = "";
        nameLabel.text = "";
        arrow.SetActive(false);

        if (dialogFrame != null)
            dialogFrame.gameObject.SetActive(false);
    }
    #endregion
}