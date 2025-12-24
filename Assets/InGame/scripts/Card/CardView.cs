using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [Header("카드 SO")]
    [SerializeField] BugCardSO bugCard;

    [Header("스프라이트 렌더러")]
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer artwork;
    [SerializeField] SpriteRenderer abilityIcon;

    [Header("텍스트")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;

    [Header("카드 앞/뒷면")]
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;

    private Card cardComponent;

    private void Awake()
    {
        cardComponent = GetComponent<Card>();
    }
    // BugCard 데이터를 세팅
    public void Setup(BugCardSO bugCard)
    {
        this.bugCard = bugCard;

        bool isFront = cardComponent != null && cardComponent.IsFront;

        if (isFront)
        {
            card.sprite = cardFront;
            artwork.sprite = bugCard.artwork;
            abilityIcon.sprite = bugCard.specialAbility != null
                ? bugCard.specialAbility.icon
                : null;

            nameText.text = bugCard.cardName;
            attackText.text = bugCard.attack.ToString();
            healthText.text = bugCard.health.ToString();
        }
        else
        {
            card.sprite = cardBack;
            artwork.sprite = null;
            abilityIcon.sprite = null;
            nameText.text = "";
            attackText.text = "";
            healthText.text = "";
        }
    }

    // 필요하다면 public 메소드로 값 갱신
    public void UpdateStats(int attack, int health)
    {
        attackText.text = attack.ToString();
        healthText.text = health.ToString();
    }
}
