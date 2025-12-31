using System.Collections.Generic;

#region 액션 메타데이터 정의
// 액션의 의미를 구분하기 위한 열거형
public enum ActionSemantic
{
    None,           // 의미 없음
    ConfirmLike,    // 확인/상호작용 계열
    CancelLike,     // 취소 계열
}

// 여러 액션을 하나의 키로 묶기 위한 그룹
public enum ActionBindGroup
{
    None,
    ConfirmInteract, // 확인과 상호작용을 묶는 그룹
}

// 액션의 추가 정보를 담는 구조체
public readonly struct ActionMeta
{
    public readonly ActionSemantic Semantic;
    public readonly ActionBindGroup Group;
    public readonly string DisplayName;   // UI 표시용 이름

    public ActionMeta(ActionSemantic semantic, ActionBindGroup group, string displayName)
    {
        Semantic = semantic;
        Group = group;
        DisplayName = displayName;
    }
}
#endregion

// 액션 ID에 대한 메타데이터를 제공하는 정적 클래스
public static class ActionDefinitions
{
    #region 필드
    // ActionId와 ActionMeta를 매핑하는 딕셔너리
    public static readonly Dictionary<ActionId, ActionMeta> Meta = new()
    {
        // Player
        { ActionId.Interact, new ActionMeta(ActionSemantic.ConfirmLike, ActionBindGroup.ConfirmInteract, "상호작용/확인") },
        { ActionId.Menu,     new ActionMeta(ActionSemantic.CancelLike,  ActionBindGroup.None,            "메뉴") },

        // UI (필요 시 Confirm 같은 ActionId 추가 후 작성)
        // { ActionId.Confirm,  new ActionMeta(ActionSemantic.ConfirmLike, ActionBindGroup.ConfirmInteract, "확인") },

        // Dialogue
        { ActionId.DialogueFastForward, new ActionMeta(ActionSemantic.None, ActionBindGroup.None, "빨리넘기기") },
    };
    #endregion

    #region 공개 메서드
    // 액션 ID에 해당하는 메타데이터를 반환
    public static ActionMeta Get(ActionId id)
        => Meta.TryGetValue(id, out var meta) ? meta : new ActionMeta(ActionSemantic.None, ActionBindGroup.None, id.ToString());
    #endregion
}