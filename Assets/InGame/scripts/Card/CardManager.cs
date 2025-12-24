using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("카드 데이터")]
    public List<BugCardSO> playerSelectedCards = new();
    public List<BugCardSO> enemyCards = new();

    [Header("카드 프리팹 및 위치")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform playerHandTransform;
    [SerializeField] Transform enemyHandTransform;
    [SerializeField] Transform playerCardSpawnPoint;
    [SerializeField] Transform enemyCardSpawnPoint;

    [Header("정렬 기준")]
    [SerializeField] Transform playerLeftAnchor;
    [SerializeField] Transform playerRightAnchor;
    [SerializeField] Transform enemyLeftAnchor;
    [SerializeField] Transform enemyRightAnchor;

    [Header("정렬 핸들러")]
    [SerializeField] CardAligner aligner;

    [Header("카드 스폰 딜레이")]
    [SerializeField] private float spawnTime;

    [Header("카드 상태")]
    [SerializeField] ECardState eCardState;

    Card selectCard;
    bool isPlayerCardDrag;
    bool onPlayerCardArea;
    enum ECardState { Noting, CanMouseOver, CanMouseDrag }
    int playerPutCount;

    private void Start()
    {
        StartCoroutine(GameFlow());
        TurnManager.OnTurnStarted += OnTurnStarted;
    }
    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }
    void OnTurnStarted(bool playerTurn)
    {
        if (playerTurn)
            playerPutCount = 0;
    }
    private void Update()
    {
        DetectCardArea();

        SetEcardState();

        if (isPlayerCardDrag)
            CardDrag();
    }
    private IEnumerator GameFlow()
    {
        // Player분배 코루틴
        yield return StartCoroutine(
                SpawnCardsSequentially(
                    playerSelectedCards,
                    playerHandTransform,
                    playerCardSpawnPoint,
                    true,
                    true
                )
            );

        // Enemy 분배 코루틴
        yield return StartCoroutine(
            SpawnCardsSequentially(
                enemyCards,
                enemyHandTransform,
                enemyCardSpawnPoint,
                false,
                false
            )
        );

        TurnManager.Instance.StartGame(); // 카드가 모두 세팅된 뒤 턴 시작
    }

    // 카드가 추가되거나 제거된 후 손패 정렬을 요청하는 메소드.
    public void RequestHandAlignment()
    {
        StartCoroutine(AlignHandsCoroutine());
    }

    private IEnumerator AlignHandsCoroutine()
    {
        // Destroy()가 실행된 후 childCount가 업데이트될 때까지 한 프레임 대기
        yield return new WaitForEndOfFrame();

        // 플레이어 손패 정렬
        var playerPRS = aligner.GetAlignment(playerHandTransform.childCount, true, playerLeftAnchor, playerRightAnchor);
        aligner.AlignCards(playerHandTransform, playerPRS);

        // 적 손패 정렬 (필요에 따라 호출)
        var enemyPRS = aligner.GetAlignment(enemyHandTransform.childCount, false, enemyLeftAnchor, enemyRightAnchor);
        aligner.AlignCards(enemyHandTransform, enemyPRS);
    }
    private IEnumerator SpawnCardsSequentially(
    List<BugCardSO> cardList,
    Transform handTransform,
    Transform spawnPoint,
    bool isPlayer,
    bool isFront)
    {
        foreach (var cardData in cardList)
        {
            SpawnCard(cardData, handTransform, spawnPoint, isFront);

            AlignHandImmediate(handTransform, isPlayer);

            yield return new WaitForSeconds(spawnTime);
        }
    }
    private void AlignHandImmediate(Transform hand, bool isPlayer)
    {
        int count = hand.childCount;
        if (count <= 0) return;

        Transform left = isPlayer ? playerLeftAnchor : enemyLeftAnchor;
        Transform right = isPlayer ? playerRightAnchor : enemyRightAnchor;

        var prsList = aligner.GetAlignment(count, isPlayer, left, right);
        aligner.AlignCards(hand, prsList);
    }

    private void SpawnCard(BugCardSO data, Transform parent, Transform spawn, bool isFront)
    {
        GameObject cardObj = Instantiate(cardPrefab, spawn.position, Quaternion.identity, parent);

        var card = cardObj.GetComponent<Card>();
        if (card != null)
        {
            card.Init(data, isFront);
        }
        else
        {
            // 디버그용
            cardObj.GetComponent<CardView>()?.Setup(data);
        }
    }
    public bool TryPutCard(bool isMine)
    {
        if (isMine)
        {
            if (playerPutCount >= 1) return false;
            // 엔티티 스폰 시도
            Vector3 spawnPos = Utils.MousePos;

            bool spawnOk = CardEntityManager.Instance.SpawnEntity(true, selectCard.Data, spawnPos);
            if (!spawnOk)
            {
                return false;
            }

            // 손패에서 카드 제거
            DestroyImmediate(selectCard.gameObject);
            selectCard = null;

            // 손패 재정렬 + 플레이어 낸 카드 수 증가
            RequestHandAlignment();
            playerPutCount++;

            return true;
        }
        else
        {
            // 적 손패에 카드 없으면 실패
            if (enemyHandTransform.childCount <= 0) return false;

            int index = Random.Range(0, enemyHandTransform.childCount);
            Transform enemyCardTf = enemyHandTransform.GetChild(index);
            Card enemyCard = enemyCardTf.GetComponent<Card>();
            if (enemyCard == null) return false;

            // 엔티티 스폰
            Vector3 spawnPos = enemyHandTransform.position;
            bool spawnOk = CardEntityManager.Instance.SpawnEntity(false, enemyCard.Data, spawnPos);
            if (!spawnOk)
            {
                return false;
            }

            Destroy(enemyCard.gameObject);
            RequestHandAlignment();

            return true;
        }
    }
    #region 카드 선택 및 드래그
    public void CardMouseOver(Card card)
    {
        selectCard = card;
        EnlargeCard(true, card);
    }
    public void CardMouseExit(Card card)
    {
        EnlargeCard(false, card);
    }
    public void CardMouseDown()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        isPlayerCardDrag = true;
    }
    public void CardMouseUp()
    {
        isPlayerCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onPlayerCardArea)
            CardEntityManager.Instance.RemoveMyEmptyEntity();
        else
            TryPutCard(true);
    }
    private void CardDrag()
    {
        if (!onPlayerCardArea)
        {
            selectCard.Motion.MoveTransForm(new PRS(Utils.MousePos, Utils.QI, selectCard.Motion.originPRS.scale), false);
            CardEntityManager.Instance.InsertPlayerEmptyEntity(Utils.MousePos.x);
        }
    }
    void EnlargeCard(bool isEnlarge, Card card)
    {
        if (isEnlarge) 
        {
            Vector3 enlargePos = new Vector3(card.Motion.originPRS.pos.x, -16.8f, -10f);
            card.Motion.MoveTransForm(new PRS(enlargePos, Utils.QI, Vector3.one * 2f),false);
        }
        else
            card.Motion.MoveTransForm(card.Motion.originPRS, false);

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }
    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("PlayerCardArea");
        onPlayerCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }
    private void SetEcardState()
    {
        if (!TurnManager.Instance.playerTurn || playerPutCount == 1 || CardEntityManager.Instance.IsFullPlayerEntities)
            eCardState = ECardState.CanMouseOver;
        else if (TurnManager.Instance.playerTurn && playerPutCount == 0)
            eCardState = ECardState.CanMouseDrag;
    }
    #endregion

}
