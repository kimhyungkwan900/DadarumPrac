using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputProvider : MonoBehaviour, IInputProvider
{
    // Enabled: InputManager보다 하위의 하드 입력 차단용
    public bool Enabled { get; set; } = true;
    public Vector2 MoveInput { get; private set; }

    [Header("키바인드(인스팩터용)")]
    [SerializeField] private List<BindingEntry> bindings = new();       // 설정된 키 List

    private Dictionary<ActionId, DualKey> map;                                // 액션아이디(입력명령), DualKey(두개의 입력 키) 으로 이루어진 Dictionary맵

    void Awake()
    {
        ApplyDefaultBindingsIfEmpty();
        BuildMap();
    }
    void Update()
    {
        if (!Enabled)
        {
            MoveInput = Vector2.zero;
            return;
        }

        float x = 0, y = 0;
        if (Get(ActionId.MoveLeft)) x -= 1;
        if (Get(ActionId.MoveRight)) x += 1;
        if (Get(ActionId.MoveDown)) y -= 1;
        if (Get(ActionId.MoveUp)) y += 1;

        // 대각선이동 정규화
        MoveInput = new Vector2(x, y).normalized;
    }

    #region 기본값 설정
    void ApplyDefaultBindingsIfEmpty()
    {
        if (bindings.Count > 0) return;

        // 이동
        SetKey(ActionId.MoveUp, BindingSlot.Primary, KeyCode.W);
        SetKey(ActionId.MoveDown, BindingSlot.Primary, KeyCode.S);
        SetKey(ActionId.MoveLeft, BindingSlot.Primary, KeyCode.A);
        SetKey(ActionId.MoveRight, BindingSlot.Primary, KeyCode.D);

        // 보조 이동
        SetKey(ActionId.MoveUp, BindingSlot.Secondary, KeyCode.UpArrow);
        SetKey(ActionId.MoveDown, BindingSlot.Secondary, KeyCode.DownArrow);
        SetKey(ActionId.MoveLeft, BindingSlot.Secondary, KeyCode.LeftArrow);
        SetKey(ActionId.MoveRight, BindingSlot.Secondary, KeyCode.RightArrow);

        // 상호작용
        SetKey(ActionId.Interact, BindingSlot.Primary, KeyCode.Space);

        // Dialogue 특수 기능
        SetKey(ActionId.DialogueFastForward, BindingSlot.Primary, KeyCode.LeftControl);

        // 메뉴
        SetKey(ActionId.Menu, BindingSlot.Primary, KeyCode.Escape);
    }
    void BuildMap()
    {
        // binding 리스트를 Dictionary 형태로 변환
        // 같은 action이 중복이면 마지막 값으로 덮어써짐
        map = new Dictionary<ActionId, DualKey>();
        foreach (var b in bindings)
            map[b.action] = b.keys;
    }
    #endregion

    #region 입력받기 && 입력값 설정
    public bool Get(ActionId action)
    {
        // Enabled 가 false 라면 무조건 false
        if (!Enabled) return false;
        // map이 null 이면 false. map 에서 해당 action의 DualKey중 하나라도 눌리면 true
        return map != null && map.TryGetValue(action, out var keys) && keys.IsPressed();
    }
    // map을 갱신하고 List를 동기화
    public void SetKey(ActionId action, BindingSlot slot, KeyCode key)
    {
        // map이 없으면 생성
        if (map == null) BuildMap();

        // map에서 해당 action의 기존 바인딩을 가져오고, 없으면 빈 바인딩 생성
        if (!map.TryGetValue(action, out var keys))
            keys = new DualKey { primary = KeyCode.None, secondary = KeyCode.None };

        // Primary/Secondary 키 세팅
        keys.Set(slot, key);
        map[action] = keys;

        // List 업데이트
        for (int i = 0; i < bindings.Count; i++)
        {
            if (bindings[i].action == action)
            {
                bindings[i].keys = keys;
                return;
            }
        }
        bindings.Add(new BindingEntry { action = action, keys = keys });
    }
    #endregion

}
