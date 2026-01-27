using System;
using UnityEngine;

// 에너지(행동 포인트) 관리 시스템
public class EnergyManager : MonoBehaviour
{
    #region 필드
    public static EnergyManager Instance { get; private set; }

    // 현재 에너지
    [SerializeField]
    private int currentEnergy = 3;

    // 최대 에너지
    [SerializeField]
    private int maxEnergy = 3;

    // 에너지 변경 시 호출되는 이벤트
    public event Action<int, int> OnEnergyChanged; // (현재, 최대)
    #endregion

    #region 초기화
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region 에너지 관리
    // 현재 에너지 반환
    public int GetCurrentEnergy() => currentEnergy;

    // 최대 에너지 반환
    public int GetMaxEnergy() => maxEnergy;

    // 에너지 소모 가능 여부 확인
    public bool CanConsume(int amount)
    {
        return currentEnergy >= amount;
    }

    // 에너지 소모
    public bool Consume(int amount)
    {
        if (!CanConsume(amount))
            return false;

        currentEnergy = Mathf.Max(0, currentEnergy - amount);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        return true;
    }

    // 에너지 회복
    public void Restore(int amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    // 에너지 최대치로 회복
    public void RestoreFull()
    {
        currentEnergy = maxEnergy;
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    // 에너지 설정
    public void SetEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(amount, 0, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    // 최대 에너지 설정
    public void SetMaxEnergy(int amount)
    {
        maxEnergy = Mathf.Max(1, amount);
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }
    #endregion
}

