using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardEntityManager : MonoBehaviour
{
    public static CardEntityManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] GameObject entityPrefab;
    [SerializeField] List<CardEntity> playerEntities;
    [SerializeField] List<CardEntity> enemyEntities;
    [SerializeField] GameObject TargetPicker;
    [SerializeField] CardEntity playerEmptyEntity;

    [SerializeField] const int MAX_ENTITY_COUNT = 1;
    public bool IsFullPlayerEntities => playerEntities.Count >= MAX_ENTITY_COUNT && !ExistPlayerEmptyEntity;
    bool IsFullEnemyEntities => enemyEntities.Count >= MAX_ENTITY_COUNT;
    bool ExistTargetPickEntity => targetPickEntity != null;
    bool ExistPlayerEmptyEntity => playerEntities.Exists(x => x == playerEmptyEntity);
    int PlayerEmptyEntityIndex => playerEntities.FindIndex(x => x == playerEmptyEntity);
    bool CanMouseInput => TurnManager.Instance.playerTurn && !TurnManager.Instance.isLoading;

    CardEntity selectEntity;
    CardEntity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);
    WaitForSeconds delay2 = new WaitForSeconds(2);
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
        AttackableReset(playerTurn);

        if (!playerTurn)
            StartCoroutine(AICo());
    }
    private void Update()
    {
        ShowTargetPicker(ExistTargetPickEntity);
    }
    IEnumerator AICo()
    {
        // 1) 적 카드 1장 내기 (손패가 없거나 필드가 꽉 차 있으면 내부에서 그냥 false 리턴)
        CardManager.Instance.TryPutCard(false);
        yield return delay1; // 카드 내는 연출

        if (enemyEntities.Count > 0 && playerEntities.Count > 0)
        {
            var attacker = enemyEntities[0];
            var target = playerEntities[0];

            // 널 / 죽은 카드 / 비어있는 자리 방어
            if (attacker != null && target != null &&
                !attacker.isDie && !attacker.isEmpty &&
                !target.isDie && !target.isEmpty &&
                attacker.attackable == true)
            {
                // 실제 공격 (DOTween 애니메이션 + 데미지 + 콜백)
                Attack(attacker, target);

                // 혹시 공격 도중 게임이 끝나면서 isLoading 이 켜졌다면 그 즉시 AI 코루틴 중단
                if (TurnManager.Instance.isLoading)
                    yield break;

                // 공격 연출이 끝날 때까지 기다리기
                yield return delay2;
            }
        }

        // 3) 턴 종료
        TurnManager.Instance.EndTurn();
    }
    void EntityAlignment(bool isMine)
    {
        float targetY = isMine ? -11f : 11.15f;
        var targetEntities = isMine ? playerEntities : enemyEntities;

        for (int i = 0; i < targetEntities.Count; i++) 
        {
            float targetX = (targetEntities.Count - 1) * -3.4f + i * 12.8f;

            var targetEntity = targetEntities[i];
            targetEntity.originPos = new Vector3(targetX, targetY, 0);
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f);
            targetEntity.GetComponent<Order>()?.SetOriginOrder(i);
        }
    }
    public void InsertPlayerEmptyEntity(float xPos)
    {
        if (IsFullPlayerEntities) return;

        if (!ExistPlayerEmptyEntity) playerEntities.Add(playerEmptyEntity);

        Vector3 emptyEntityPos = playerEmptyEntity.transform.position;
        emptyEntityPos.x = xPos;
        playerEmptyEntity.transform.position = emptyEntityPos;

        int _emptyEntityIndex = PlayerEmptyEntityIndex;
        playerEntities.Sort((entity1, entity2) => entity1.transform.position.x.CompareTo(entity2.transform.position.x));
        if (PlayerEmptyEntityIndex != _emptyEntityIndex) EntityAlignment(true);
    }
    public void RemoveMyEmptyEntity()
    {
        if (!ExistPlayerEmptyEntity) return;

        playerEntities.RemoveAt(PlayerEmptyEntityIndex);
        EntityAlignment(true);
    }
    public bool SpawnEntity(bool isMine, BugCardSO bugCard, Vector3 spawnPos)
    {
        if (isMine)
        {
            if (IsFullPlayerEntities || !ExistPlayerEmptyEntity)
            {
                return false;
            }
        }
        else
        {
            if (IsFullEnemyEntities)
            {
                return false;
            }
        }

        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityObject.GetComponent<CardEntity>();

        if (isMine) 
            playerEntities[PlayerEmptyEntityIndex] = entity;
        else 
            enemyEntities.Insert(Random.Range(0, enemyEntities.Count), entity);

        entity.isMine = isMine;
        entity.Setup(bugCard);
        EntityAlignment(isMine);

        return true;
    }
    #region 공격
    // 공격 로직
    void Attack(CardEntity attacker, CardEntity target)
    {
        attacker.attackable = false;
        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Sequence sequence = DOTween.Sequence()
            .Append(attacker.transform.DOMove(target.originPos, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() =>
            {
                attacker.Damaged(target.attack);
                target.Damaged(attacker.attack);
            })
            .Append(attacker.transform.DOMove(attacker.originPos, 0.4f)).SetEase(Ease.OutSine)
            .OnComplete(() => AttackCallback(attacker, target)); // 죽음 처리
    }

    void AttackCallback(params CardEntity[] entities)
    {
        // 죽을 Entity를 골라 죽음 처리
        entities[0].GetComponent<Order>().SetMostFrontOrder(false);

        foreach ( var entity in entities)
        {
            if (!entity.isDie || entity.isEmpty)
                continue;

            if(entity.isMine)
                playerEntities.Remove(entity);
            else
                enemyEntities.Remove(entity);

            // 죽음 모션 DOTween
            Sequence sequence = DOTween.Sequence()
                .Append(entity.transform.DOShakePosition(1.3f))
                .Append(entity.transform.DOScale(Vector3.zero, 0.3f)).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    EntityAlignment(entity.isMine);
                    Destroy(entity.gameObject);
                });
        }
    }

    // 타겟픽커 보여주기
    private void ShowTargetPicker(bool isShow)
    {
        TargetPicker.SetActive(isShow);
        if (ExistTargetPickEntity)
            TargetPicker.transform.position = targetPickEntity.transform.position;
    }

    // 공격가능으로 초기화 (턴 시작시)
    public void AttackableReset(bool isMine)
    {
        var targetEntites = isMine ? playerEntities : enemyEntities;
        targetEntites.ForEach(x => x.attackable = true);
    }
    #endregion

    #region 엔티티 선택 및 드래그
    public void EntityMouseDown(CardEntity entity)
    {
        if (!CanMouseInput)
            return;

        selectEntity = entity;
    }
    public void EntityMouseUp()
    {
        if (!CanMouseInput)
            return;

        if (selectEntity && targetPickEntity && selectEntity.attackable)
            Attack(selectEntity, targetPickEntity);

        selectEntity = null;
        targetPickEntity = null;
    }
    public void EntityMouseDrag()
    {
        if(!CanMouseInput || selectEntity == null)
            return;

        bool existTarget = false;
        foreach (var hit in Physics2D.RaycastAll(Utils.MousePos, Vector3.forward))
        {
            CardEntity entity = hit.collider?.GetComponent<CardEntity>();
            if(entity != null && !entity.isMine && selectEntity.attackable)
            {
                targetPickEntity = entity;
                existTarget = true;
                break;
            }
        }
        if (!existTarget)
            targetPickEntity = null;
    }
    #endregion
}
