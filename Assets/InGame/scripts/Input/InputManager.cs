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

// 입력 시스템을 관리하는 싱글톤 매니저. 컨텍스트 기반으로 입력을 제어합니다.
public class InputManager : MonoBehaviour
{
    #region 컴포넌트
    [Header("Provider")]
    [SerializeField]
    private PlayerInputProvider playerInput;

    [Header("Context Policies")]
    [SerializeField]
    private InputContextPolicy[] contextPolicies;
    #endregion

    #region 필드
    public static InputManager Instance { get; private set; }
    
    // 컨텍스트 변경 시 호출되는 이벤트
    public event Action<InputContext> OnContextChanged;

    // 전역 입력 활성화 여부
    public bool GlobalEnabled { get; private set; } = true;

    // 현재 활성화된 컨텍스트
    public InputContext CurrentContext { get; private set; } = InputContext.Player;
    
    // 컨텍스트 스택
    private readonly Stack<InputContext> contextStack = new();
    
    // 컨텍스트별 허용된 액션 맵
    private Dictionary<InputContext, HashSet<ActionId>> contextActionMap;
    #endregion

    #region 초기화
    private void Awake()
    {
        // 싱글톤 패턴 설정
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

    // 컨텍스트별 액션 맵 빌드
    private void BuildContextActionMap()
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
    #endregion

    #region 유니티 생명주기
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    #region 컨텍스트 관리
    // 전역 입력 활성화/비활성화 설정
    public void SetGlobalEnabled(bool enabled)
    {
        GlobalEnabled = enabled;
        ApplyContext(CurrentContext);
    }

    // 컨텍스트 직접 변경 (스택 사용 안함)
    public void SetContext(InputContext context)
    {
        if (CurrentContext == context) return;
        
        CurrentContext = context;
        ApplyContext(context);
        OnContextChanged?.Invoke(context);
    }

    // 새 컨텍스트를 스택에 추가하고 전환
    public void PushContext(InputContext context)
    {
        contextStack.Push(CurrentContext);
        SetContext(context);
    }

    // 이전 컨텍스트로 복원
    public void PopContext()
    {
        if (contextStack.Count > 0)
            SetContext(contextStack.Pop());
        else
            Debug.LogWarning("[InputManager] PopContext: 스택이 비어있습니다.");
    }
    #endregion

    #region 입력 처리
    // 특정 액션의 입력 상태 확인
    public bool GetAction(ActionId action)
    {
        if (!GlobalEnabled || playerInput == null || !IsActionAllowedInContext(action, CurrentContext))
            return false;

        return playerInput.Get(action);
    }

    // 이동 입력 값 반환
    public Vector2 MoveInput
    {
        get
        {
            if (!GlobalEnabled || CurrentContext != InputContext.Player || playerInput == null)
                return Vector2.zero;
            
            return playerInput.MoveInput;
        }
    }
    #endregion

    #region 내부 로직
    // 특정 컨텍스트에서 액션 허용 여부 확인
    private bool IsActionAllowedInContext(ActionId action, InputContext ctx)
    {
        if (!contextActionMap.TryGetValue(ctx, out var set))
        {
            Debug.LogWarning($"[InputManager] 입력 정책 없음: {ctx}");
            return false;
        }
        return set.Contains(action);
    }

    // 현재 컨텍스트 적용
    private void ApplyContext(InputContext context)
    {
        if (playerInput != null)
            playerInput.Enabled = GlobalEnabled;
    }
    #endregion
}
