using System;
using System.Collections.Generic;
using UnityEngine;

// TODO [SaveLoad]: 파일 저장/로드 구현
// TODO [Migration]: SaveData 버전 관리 및 마이그레이션
// TODO [Progress]: 챕터/엔딩/트리거 진행도
// TODO [Debug]: 디버그 세이브 도구
[Serializable]
public class SaveData
{
    public string currentScene;
    public Vector3 playerPosition;

    public Dictionary<string, bool> flags = new();
    public Dictionary<string, int> variables = new();

}

public class SaveManager : MonoBehaviour
{
    #region 필드

    public static SaveManager I { get; private set; }
    public SaveData Data { get; private set; } = new SaveData();

    #endregion

    #region 초기화

    // 싱글톤 초기화 및 DontDestroyOnLoad 설정
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region 게임 초기화

    // 새 게임 시작 시 데이터 초기화
    public void NewGame()
    {
        Data.flags.Clear();
        Data.variables.Clear();
        // TODO: 게임 초기 설정
    }

    #endregion

    #region Flag 관리

    // flag 값 반환
    public bool GetFlag(string key) =>
        Data.flags.TryGetValue(key, out var v) && v;

    // flag 설정
    public void SetFlag(string key, bool value = true) =>
        Data.flags[key] = value;

    // flag 존재 여부 확인
    public bool HasFlag(string key)
    {
        return Data.flags.ContainsKey(key);
    }

    #endregion

    #region Variable 관리

    // variable 값 반환
    public int GetVar(string key) =>
        Data.variables.TryGetValue(key, out var v) ? v : 0;

    // variable 설정
    public void SetVar(string key, int value)
    {
        Data.variables[key] = value;
        Debug.Log($"SetVar: {key} = {value}");
    }

    // variable 값 증가/감소
    public void AddVar(string key, int delta)
    {
        SetVar(key, GetVar(key) + delta);
    }

    #endregion

    #region Item 관리

    // 아이템 수량 반환
    public int GetItemCount(string itemKey) => GetVar($"item_{itemKey}");

    // 아이템 수량 설정 및 flag 자동 업데이트
    public void SetItemCount(string itemKey, int count, int threshold = 0, string flagKey = null)
    {
        SetVar($"item_{itemKey}", count);
        
        if (threshold > 0)
        {
            UpdateItemFlag(itemKey, threshold, flagKey);
        }
    }

    // 아이템 수량 추가 및 flag 자동 업데이트
    public void AddItemCount(string itemKey, int delta, int threshold = 0, string flagKey = null)
    {
        int newCount = GetItemCount(itemKey) + delta;
        SetItemCount(itemKey, newCount, threshold, flagKey);
    }

    // 아이템 수량이 threshold 이상인지 확인
    public bool HasEnoughItem(string itemKey, int threshold)
    {
        return GetItemCount(itemKey) >= threshold;
    }

    // 아이템 수량에 따라 flag 업데이트
    private void UpdateItemFlag(string itemKey, int threshold, string flagKey = null)
    {
        if (string.IsNullOrEmpty(flagKey))
        {
            flagKey = $"has_enough_{itemKey}";
        }

        bool hasEnough = HasEnoughItem(itemKey, threshold);
        SetFlag(flagKey, hasEnough);
    }

    #endregion
}
