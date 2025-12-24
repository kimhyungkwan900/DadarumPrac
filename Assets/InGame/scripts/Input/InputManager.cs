using System;
using System.Collections.Generic;
using UnityEngine;
public enum InputContext
{
    Player,
    Dialogue,
    UI,
    Cutscene
}
public class InputManager : MonoBehaviour
{
    #region 변수
    public static InputManager Instance { get; private set; }
    public event Action<InputContext> OnContextChanged; // Context 변경 이벤트 알림

    [Header("Provider")]
    [SerializeField] private PlayerInputProvider playerInput;

    [Header("Context Policies")]
    [SerializeField] private InputContextPolicy[] contextPolicies;
    // 전역잠금
    public bool GlobalEnabled { get; private set; } = true;

    readonly Stack<InputContext> contextStack = new();
    public InputContext CurrentContext { get; private set; } = InputContext.Player;

    Dictionary<InputContext, HashSet<ActionId>> contextActionMap;
    #endregion
    private void Awake()
    {
       if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildContextActionMap();
        ApplyContext(CurrentContext);
    }

    #region 상태관리 (이동 / UI / 대화문 / 컷씬 등)
    public void SetGlobalEnabled(bool enabled)
    {
        GlobalEnabled = enabled;
        ApplyContext(CurrentContext);
    }

    public void SetContext(InputContext context)
    {
        if (CurrentContext == context) return;
        CurrentContext = context;

        ApplyContext(context);
        OnContextChanged?.Invoke(context);
    }

    public void PushContext(InputContext context)
    {
        contextStack.Push(CurrentContext);
        SetContext(context);
    }

    public void PopContext()
    {
        if (contextStack.Count > 0)
            SetContext(contextStack.Pop());
    }
    #endregion

    #region 입력처리
    // 플레이어 입력을 확인후 해당 동작 실행
    public bool GetAction(ActionId action)
    {
        if (!GlobalEnabled) return false;
        if (playerInput == null) return false;

        if (!IsActionAllowedInContext(action, CurrentContext))
            return false;

        return playerInput.Get(action);
    }

    // 이동 입력이 들어왔을때 GlobalEnabled 가 True 이고 현재 입력이 Player일때 PlayerInput 이 있다면 MoveInput 을 수행
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

    #region 상태관리 도우미
    void BuildContextActionMap()
    {
        contextActionMap = new();

        foreach (var policy in contextPolicies)
        {
            if (contextActionMap.ContainsKey(policy.context))
            {
                Debug.LogWarning($"중복된 상태입력: {policy.context}");
                continue;
            }

            contextActionMap.Add(
                policy.context,
                new HashSet<ActionId>(policy.allowedActions)
            );
        }
    }
    bool IsActionAllowedInContext(ActionId action, InputContext ctx)
    {
        if (!contextActionMap.TryGetValue(ctx, out var set))
        {
            Debug.LogWarning($"[InputManager] 입력상태 누락: {ctx}");
            return false;
        }
        return set.Contains(action);
    }

    void ApplyContext(InputContext context)
    {
        // Context 변경 시 호출되지만 실제 입력 허용 여부는 GlobalEnabled + Policy에서 판단한다.
        if (playerInput != null)
            playerInput.Enabled = GlobalEnabled;
    }
    #endregion
}
