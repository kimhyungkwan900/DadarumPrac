using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private BugCardSO bugCard;
    [SerializeField] private bool isFront = true;

    public CardView View { get; private set; }
    public CardMotion Motion { get; private set; }

    public bool IsFront => isFront;
    public BugCardSO Data => bugCard;

    private void Awake()
    {
        View = GetComponent<CardView>();
        Motion = GetComponent<CardMotion>();
    }

    // 카드 데이터와 앞/뒷면을 초기화.
    public void Init(BugCardSO data, bool isFront)
    {
        this.bugCard = data;
        this.isFront = isFront;

        // View는 항상 Card의 isFront를 기준으로 그림
        if (View == null)
            View = GetComponent<CardView>();

        View.Setup(bugCard);
    }

    // 이후에 카드를 뒤집고 싶을 때 사용할 수 있는 함수 (옵션).
    public void SetFront(bool isFront)
    {
        this.isFront = isFront;

        if (View == null)
            View = GetComponent<CardView>();

        View.Setup(bugCard);
    }

    private void OnMouseOver()
    {
        if (isFront)
            CardManager.Instance.CardMouseOver(this);
    }

    private void OnMouseExit()
    {
        if (isFront)
            CardManager.Instance.CardMouseExit(this);
    }

    private void OnMouseDown()
    {
        if (isFront)
            CardManager.Instance.CardMouseDown();
    }

    private void OnMouseUp()
    {
        if (isFront)
            CardManager.Instance.CardMouseUp();
    }
}
