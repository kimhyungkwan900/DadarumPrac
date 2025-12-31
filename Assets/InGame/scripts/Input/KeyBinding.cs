using System;
using UnityEngine;

#region 입력 액션 ID
// 입력으로 수행할 수 있는 모든 액션을 정의하는 열거형
public enum ActionId
{
    // 플레이어
    MoveUp, 
    MoveDown, 
    MoveLeft,
    MoveRight,

    // 상호작용
    Interact, 

    // 메뉴
    Menu,

    // 대화
    DialogueFastForward, // 빨리감기
}
#endregion

#region 키 바인딩 데이터 구조
// 기본 키, 보조 키를 구분하기 위한 열거형
public enum BindingSlot { Primary = 0, Secondary = 1 }

// 기본 키와 보조 키를 한 쌍으로 관리하는 구조체
[Serializable]
public struct DualKey
{
    public KeyCode primary;
    public KeyCode secondary;

    // 두 키 중 하나라도 눌렸는지 확인
    public bool IsPressed()
    {
        return (primary != KeyCode.None && Input.GetKey(primary)) ||
               (secondary != KeyCode.None && Input.GetKey(secondary));
    }

    // 특정 슬롯의 키 코드를 반환
    public KeyCode Get(BindingSlot slot) => slot == BindingSlot.Primary ? primary : secondary;

    // 특정 슬롯에 키 코드를 설정
    public void Set(BindingSlot slot, KeyCode key)
    {
        if (slot == BindingSlot.Primary) primary = key;
        else secondary = key;
    }
}

// ActionId와 DualKey를 연결하는 데이터 클래스 (인스펙터 표시용)
[Serializable]
public class BindingEntry
{
    public ActionId action;
    public DualKey keys;
}
#endregion
