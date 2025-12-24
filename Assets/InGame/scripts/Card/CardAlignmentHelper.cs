using System.Collections.Generic;
using UnityEngine;

public static class CardAlignmentHelper
{
    public static List<PRS> RoundAlignment(
    Transform leftTr,
    Transform rightTr,
    int objCount,
    float height = 0.5f,
    bool reverse = false)
    {
        if (objCount <= 0) return new List<PRS>();

        Vector3 scale = Vector3.one;
        List<PRS> results = new List<PRS>(objCount);

        float[] objLerps = new float[objCount];

        switch (objCount)
        {
            case 1:
                objLerps = new float[] { 0.5f };
                break;
            case 2:
                objLerps = new float[] { 0.27f, 0.73f };
                break;
            case 3:
                objLerps = new float[] { 0.1f, 0.5f, 0.9f };
                break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                {
                    objLerps[i] = interval * i;
                }
                break;
        }

        float centerIndex = (objCount - 1) / 2f;

        for (int i = 0; i < objCount; i++)
        {
            int lerpIndex = reverse ? (objCount - 1 - i) : i;

            float lerp = objLerps[lerpIndex];

            // 위치 계산
            Vector3 pos = Vector3.Lerp(leftTr.position, rightTr.position, lerp);

            // 곡률 적용 (중앙 기준 대칭)
            float offset = lerpIndex - centerIndex;
            float curvedY = -Mathf.Pow(offset, 2) * height + height;
            pos.y += curvedY;

            // 회전 적용
            float maxAngle = Mathf.Clamp(20f - objCount, 5f, 20f);
            float angle = Mathf.Lerp(20f, -20f, lerp); // 중앙일수록 0도
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public static List<PRS> FixedPositionAlignment(int count, Transform t1, Transform t2, Transform t3)
    {
        List<PRS> result = new();

        if (count == 1) result.Add(new PRS(t2.position, t2.rotation, Vector3.one));
        else if (count == 2)
        {
            result.Add(new PRS(t1.position, t1.rotation, Vector3.one));
            result.Add(new PRS(t3.position, t3.rotation, Vector3.one));
        }
        else if (count == 3)
        {
            result.Add(new PRS(t1.position, t1.rotation, Vector3.one));
            result.Add(new PRS(t2.position, t2.rotation, Vector3.one));
            result.Add(new PRS(t3.position, t3.rotation, Vector3.one));
        }

        return result;
    }
}