using System.Collections.Generic;

public enum ActionSemantic
{
    None,              // 미지정
    ConfirmLike,   // 상호작용(확인) 버튼
    CancelLike,     // 취소 버튼
}

public enum ActionBindGroup
{
    None,
    ConfirmInteract, // UI상 묶음
}

public readonly struct ActionMeta
{
    public readonly ActionSemantic Semantic;
    public readonly ActionBindGroup Group;
    public readonly string DisplayName;   // UI 표시용이름

    public ActionMeta(ActionSemantic semantic, ActionBindGroup group, string displayName)
    {
        Semantic = semantic;
        Group = group;
        DisplayName = displayName;
    }
}

// 데이터 확장 수정용
public static class ActionDefinitions
{
    public static readonly Dictionary<ActionId, ActionMeta> Meta = new()
    {
        // Player
        { ActionId.Interact, new ActionMeta(ActionSemantic.ConfirmLike, ActionBindGroup.ConfirmInteract, "상호작용/확인") },
        { ActionId.Menu,     new ActionMeta(ActionSemantic.CancelLike,  ActionBindGroup.None,            "메뉴") },

        // UI (Confirm 같은 ActionId 추가시 작성)
        // { ActionId.Confirm,  new ActionMeta(ActionSemantic.ConfirmLike, ActionBindGroup.ConfirmInteract, "확인") },

        // Dialogue
        { ActionId.DialogueFastForward, new ActionMeta(ActionSemantic.None, ActionBindGroup.None, "빨리감기") },
    };

    public static ActionMeta Get(ActionId id)
        => Meta.TryGetValue(id, out var meta) ? meta : new ActionMeta(ActionSemantic.None, ActionBindGroup.None, id.ToString());
}
