using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardEntity : MonoBehaviour
{
    [SerializeField] BugCardSO bugCard;
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer artwork;
    [SerializeField] SpriteRenderer ability;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;

    public int attack;
    public int health;
    public bool isMine;
    public bool isDie;
    public bool isEmpty;
    public Vector3 originPos;
    public BugCardSO Data => bugCard;
    public bool attackable;
    int liveCount;
    private void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;
    }
    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }
    void OnTurnStarted(bool playerTurn)
    {
        if (isEmpty)
            return;

        if (isMine == playerTurn)
            liveCount++;
    }
    public void Setup(BugCardSO bugCard)
    {
        this.bugCard = bugCard;

        attack = bugCard.attack;
        health = bugCard.health;

        artwork.sprite = this.bugCard.artwork;
        ability.sprite = this.bugCard.specialAbility != null
            ? this.bugCard.specialAbility.icon
            : null;

        nameTMP.text = this.bugCard.cardName;
        attackTMP.text = this.bugCard.attack.ToString();
        healthTMP.text = this.bugCard.health.ToString();
    }

    // 데미지 계산 로직
    public bool Damaged(int damage)
    {
        health -= damage;
        healthTMP.text = health.ToString();

        if (health <= 0)
        {
            isDie = true;

            // 죽었을시 카운트 로직

            return true;
        }

        return false;
    }

    #region 마우스 다운 업 드래그
    void OnMouseDown()
    {
        if (isMine)
            CardEntityManager.Instance.EntityMouseDown(this);
    }
    void OnMouseUp()
    {
        if (isMine)
            CardEntityManager.Instance.EntityMouseUp();
    }
    void OnMouseDrag()
    {
        if (isMine)
            CardEntityManager.Instance.EntityMouseDrag();
    }
    #endregion

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween) 
        {
            transform.DOMove(pos,dotweenTime);
        }
        else
        {
            transform.position = pos;
        }
    }

}
