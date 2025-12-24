using System;
using UnityEngine;

public enum ActionId
{
    // 플레이어
    // 이동
    MoveUp, 
    MoveDown, 
    MoveLeft,
    MoveRight,

    // 상호작용
    Interact, 

    // 메뉴
    Menu,

    // 대화문 입력
    DialogueFastForward,
}

public enum BindingSlot { Primary = 0, Secondary = 1 }

[Serializable]
public struct DualKey
{
    public KeyCode primary;
    public KeyCode secondary;

    public bool IsPressed()
    {
        return (primary != KeyCode.None && Input.GetKey(primary)) ||
               (secondary != KeyCode.None && Input.GetKey(secondary));
    }

    public KeyCode Get(BindingSlot slot) => slot == BindingSlot.Primary ? primary : secondary;

    public void Set(BindingSlot slot, KeyCode key)
    {
        if (slot == BindingSlot.Primary) primary = key;
        else secondary = key;
    }
}

[Serializable]
public class BindingEntry
{
    public ActionId action;
    public DualKey keys;
}