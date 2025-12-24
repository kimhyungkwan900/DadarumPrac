using UnityEngine;
using DG.Tweening;

public class CardMotion : MonoBehaviour
{
    [SerializeField] private Order order;

    // 원래 위치/회전/스케일
    public PRS originPRS;
    private bool initialized;

    public void SetOriginPRS(PRS prs)
    {
        originPRS = prs;
        initialized = true;
    }

    // 카드 이동/회전/스케일 트윈
    public void MoveTransForm(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            DOTween.Kill(transform);

            transform.DOMove(prs.pos, dotweenTime).SetEase(Ease.OutCubic);
            transform.DORotateQuaternion(prs.rot, dotweenTime).SetEase(Ease.OutCubic);
            transform.DOScale(
                prs.scale == Vector3.zero ? Vector3.one : prs.scale,
                dotweenTime
            ).SetEase(Ease.OutBack);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale == Vector3.zero ? Vector3.one : prs.scale;
        }
    }
}
