using System;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 입력을 받아 처리하는 클래스 (키보드)
public class PlayerInputProvider : MonoBehaviour, IInputProvider
{
    #region 컴포넌트
    [Header("키 바인딩 (인스펙터용)")]
    [SerializeField] private List<BindingEntry> bindings = new();
    #endregion

    #region 프로퍼티
    // 입력 활성화 여부 (InputManager에 의해 제어)
    public bool Enabled { get; set; } = true;
    
    // 정규화된 이동 입력 값
    public Vector2 MoveInput { get; private set; }
    #endregion

    #region 필드
    // ActionId와 DualKey를 매핑하는 딕셔너리
    private Dictionary<ActionId, DualKey> map;
    #endregion

    #region 초기화
    private void Awake()
    {
        ApplyDefaultBindingsIfEmpty();
        BuildMap();
    }
    #endregion

    #region 유니티 생명주기
    private void Update()
    {
        if (!Enabled)
        {
            MoveInput = Vector2.zero;
            return;
        }

        // 이동 입력 계산
        float x = 0, y = 0;
        if (Get(ActionId.MoveLeft)) x -= 1;
        if (Get(ActionId.MoveRight)) x += 1;
        if (Get(ActionId.MoveDown)) y -= 1;
        if (Get(ActionId.MoveUp)) y += 1;

        // 대각선 이동 정규화
        MoveInput = new Vector2(x, y).normalized;
    }
    #endregion

    #region 키 바인딩 관리
    // 특정 액션의 입력 상태 확인
    public bool Get(ActionId action)
    {
        if (!Enabled || map == null) return false;
        
        return map.TryGetValue(action, out var keys) && keys.IsPressed();
    }

    // 키 바인딩 설정
    public void SetKey(ActionId action, BindingSlot slot, KeyCode key)
    {
        if (map == null) BuildMap();

        if (!map.TryGetValue(action, out var keys))
            keys = new DualKey { primary = KeyCode.None, secondary = KeyCode.None };

        keys.Set(slot, key);
        map[action] = keys;

        // 인스펙터용 List 업데이트
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

    #region 내부 로직
    // 키 바인딩이 비어있을 경우 기본값 적용
    private void ApplyDefaultBindingsIfEmpty()
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

        // 대화 빨리감기
        SetKey(ActionId.DialogueFastForward, BindingSlot.Primary, KeyCode.LeftControl);

        // 메뉴
        SetKey(ActionId.Menu, BindingSlot.Primary, KeyCode.Escape);
    }

    // 인스펙터의 바인딩 리스트를 딕셔너리로 변환
    private void BuildMap()
    {
        map = new Dictionary<ActionId, DualKey>();
        foreach (var b in bindings)
            map[b.action] = b.keys;
    }
    #endregion
}