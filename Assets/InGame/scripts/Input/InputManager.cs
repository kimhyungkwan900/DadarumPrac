using System;
using System.Collections.Generic;
using UnityEngine;

// 입력 컨텍스트 타입
public enum InputContext
{
    Player,     // 플레이어 이동/액션
    Dialogue,   // 대화
    UI,         // UI 메뉴
    Cutscene    // 컷씬
}

// 입력 시스템을 관리하는 싱글톤 매니저 컨텍스트 기반으로 입력을 제어
public class InputManager : MonoBehaviour
{
    #region 필드
    public static InputManager Instance { get; private set; }
    
    // 컨텍스트 변경 이벤트
    public event Action<InputContext> OnContextChanged;

    [Header("Provider")]
    [SerializeField]
    private PlayerInputProvider playerInput;

    [Header("Context Policies")]
    [SerializeField]
    private InputContextPolicy[] contextPolicies;

    // 전역 입력 활성화 여부
    public bool GlobalEnabled { get; private set; } = true;

    // 컨텍스트 스택 (Push/Pop을 위한)
    readonly Stack<InputContext> contextStack = new();
    
    // 현재 활성화된 컨텍스트
    public InputContext CurrentContext { get; private set; } = InputContext.Player;

    // 컨텍스트별 허용된 액션 맵
    Dictionary<InputContext, HashSet<ActionId>> contextActionMap;
    #endregion

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildContextActionMap();
        ApplyContext(CurrentContext);
    }

    #region 컨텍스트 관리 (이동 / UI / 대화 / 컷씬 등)
    
    // 전역 입력 활성화/비활성화
    public void SetGlobalEnabled(bool enabled)
    {
        GlobalEnabled = enabled;
        ApplyContext(CurrentContext);
    }

    // 컨텍스트 설정 (스택 없이 직접 변경)
    public void SetContext(InputContext context)
    {
        if (CurrentContext == context) return;
        
        CurrentContext = context;
        ApplyContext(context);
        OnContextChanged?.Invoke(context);
    }

    // 컨텍스트를 스택에 Push (현재 컨텍스트를 저장하고 새 컨텍스트로 전환)
    public void PushContext(InputContext context)
    {
        contextStack.Push(CurrentContext);
        SetContext(context);
    }

    // 컨텍스트를 스택에서 Pop (이전 컨텍스트로 복원)
    public void PopContext()
    {
        if (contextStack.Count > 0)
            SetContext(contextStack.Pop());
        else
            Debug.LogWarning("[InputManager] PopContext: 스택이 비어있습니다.");
    }
    #endregion

    #region 입력 처리
    
    // 플레이어 입력을 확인하여 해당 컨텍스트에서 허용된 액션인지 검사하여 반환
    public bool GetAction(ActionId action)
    {
        if (!GlobalEnabled) return false;
        if (playerInput == null) return false;

        if (!IsActionAllowedInContext(action, CurrentContext))
            return false;

        return playerInput.Get(action);
    }

    // 이동 입력을 반환
    // GlobalEnabled가 True이고 현재 입력 컨텍스트가 Player일 때만 PlayerInput이 존재하면 MoveInput을 반환
    public Vector2 MoveInput
    {
        get
        {
            if (!GlobalEnabled) return Vector2.zero;
            if (CurrentContext != InputContext.Player) return Vector2.zero;
            return playerInput != null ? playerInput.MoveInput : Vector2.zero;
        }
    }
    #endregion

    #region 컨텍스트 정책 관리
    
    // 컨텍스트별 액션 맵을 빌드
    void BuildContextActionMap()
    {
        contextActionMap = new();

        foreach (var policy in contextPolicies)
        {
            if (policy == null) continue;

            if (contextActionMap.ContainsKey(policy.context))
            {
                Debug.LogWarning($"중복된 컨텍스트 정책: {policy.context}");
                continue;
            }

            contextActionMap.Add(
                policy.context,
                new HashSet<ActionId>(policy.allowedActions)
            );
        }
    }

    // 특정 컨텍스트에서 액션이 허용되는지 확인
    bool IsActionAllowedInContext(ActionId action, InputContext ctx)
    {
        if (!contextActionMap.TryGetValue(ctx, out var set))
        {
            Debug.LogWarning($"[InputManager] 입력 정책 없음: {ctx}");
            return false;
        }
        return set.Contains(action);
    }

    // 컨텍스트를 적용합니다
    void ApplyContext(InputContext context)
    {
        if (playerInput != null)
            playerInput.Enabled = GlobalEnabled;
    }
    #endregion


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
