using System.Collections.Generic;
using UnityEngine;

public class CardAligner : MonoBehaviour
{
    [Header("플레이어 고정 포지션 (1~3장)")]
    [SerializeField] private Transform[] playerSlots = new Transform[3];

    [Header("적 고정 포지션 (1~3장)")]
    [SerializeField] private Transform[] enemySlots = new Transform[3];

    [Header("커브/트윈 설정")]
    [SerializeField] private float playerCurveDirection = 0.6f;   // 플레이어 손패 곡률
    [SerializeField] private float enemyCurveDirection = -0.6f;   // 적 손패 곡률
    [SerializeField] private float alignTweenDuration = 0.25f;    // 카드 정렬 트윈 시간

    // 카드 개수와 진영에 따라 정렬용 PRS 리스트를 계산.
    // 1~3장은 고정 포지션, 그 이상은 라운드(곡선) 배치.
    public List<PRS> GetAlignment(int cardCount, bool isPlayer, Transform left, Transform right)
    {
        if (cardCount <= 0)
            return new List<PRS>();

        // 3장 이하 : 미리 세팅된 고정 포지션 사용
        if (cardCount <= 3)
        {
            Transform[] slots = isPlayer ? playerSlots : enemySlots;

            // 슬롯이 3개 다 안 채워져 있어도 N개까진 방어적으로 처리
            Transform s1 = slots.Length > 0 ? slots[0] : null;
            Transform s2 = slots.Length > 1 ? slots[1] : s1;
            Transform s3 = slots.Length > 2 ? slots[2] : s2;

            return CardAlignmentHelper.FixedPositionAlignment(
                cardCount,
                s1,
                s2,
                s3
            );
        }

        // 4장 이상 : 곡선 배치
        float curve = isPlayer ? playerCurveDirection : enemyCurveDirection;
        bool isReversed = !isPlayer;

        return CardAlignmentHelper.RoundAlignment(
            left,
            right,
            cardCount,
            curve,
            isReversed
        );
    }

    // 실제 손패 Transform 하위에 있는 카드들을 주어진 PRS 리스트에 맞게 정렬.
    public void AlignCards(Transform hand, List<PRS> positions)
    {
        if (hand == null || positions == null)
            return;

        // 방어 코드: 카드 수와 PRS 수 중 작은 값까지만 처리
        int cardCount = Mathf.Min(hand.childCount, positions.Count);

        for (int i = 0; i < cardCount; i++)
        {
            Transform cardTransform = hand.GetChild(i);

            CardMotion motion = cardTransform.GetComponent<CardMotion>();
            Order order = cardTransform.GetComponent<Order>();

            PRS targetPRS = positions[i];

            if (motion != null)
            {
                motion.SetOriginPRS(targetPRS);
                motion.MoveTransForm(targetPRS, true, alignTweenDuration);
            }

            if (order != null)
            {
                order.SetOriginOrder(i);
            }
        }
    }
}
