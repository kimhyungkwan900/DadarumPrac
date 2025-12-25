using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

// 턴 기반 전투 시스템을 관리합니다.
// 플레이어와 적의 턴을 제어하고, 턴 전환을 담당합니다.
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [Header("Develop")]
    [SerializeField][Tooltip("게임 시작 시 턴을 설정합니다")] 
    private ETurnMode eTurnMode = ETurnMode.Random;

    [Header("Properties")]
    public bool playerTurn;
    public bool isLoading;
    public int currentTurnCount { get; private set; }

    [Header("Turn Delay")]
    [SerializeField] private float turnStartDelay = 0.3f;  // 턴 시작 전 대기 시간
    [SerializeField] private float turnEndDelay = 0.3f;    // 턴 종료 후 대기 시간

    [Header("Turn Limits")]
    [SerializeField] private int maxTurnCount = 50;        // 최대 턴 수 (무한 루프 방지)

    // 턴 모드 설정
    private enum ETurnMode 
    { 
        Random,  // 무작위로 선택
        Player,  // 플레이어 선공
        Enemy    // 적 선공
    }

    // 턴 상태
    private enum TurnState 
    { 
        Waiting,      // 대기 중
        PlayerTurn,   // 플레이어 턴
        EnemyTurn,    // 적 턴
        Ended         // 턴 종료
    }

    private TurnState currentTurnState = TurnState.Waiting;

    // 이벤트
    public static event Action<bool> OnTurnStarted;      // 턴 시작 (true: 플레이어, false: 적)
    public static event Action<bool> OnTurnEnded;        // 턴 종료
    public static event Action OnGameStarted;            // 게임 시작
    public static event Action OnMaxTurnReached;         // 최대 턴 도달

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 게임 초기 설정
    void GameSetup()
    {
        currentTurnCount = 0;
        
        switch (eTurnMode)
        {
            case ETurnMode.Random: 
                playerTurn = Random.Range(0, 2) == 0; 
                break;
            case ETurnMode.Player: 
                playerTurn = true; 
                break;
            case ETurnMode.Enemy: 
                playerTurn = false; 
                break;
        }

        Debug.Log($"게임 시작 - {(playerTurn ? "플레이어" : "적")} 선공");
    }

    // 게임 시작
    public void StartGame()
    {
        if (isLoading)
        {
            Debug.LogWarning("TurnManager: 이미 게임이 시작 중입니다.");
            return;
        }

        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        isLoading = true;
        currentTurnState = TurnState.Waiting;

        GameSetup();
        OnGameStarted?.Invoke();

        // 바로 첫 턴 시작 코루틴으로 연결
        yield return StartCoroutine(TurnStartRoutine());
    }

    // 턴 시작 코루틴
    IEnumerator TurnStartRoutine()
    {
        isLoading = true;

        // 최대 턴 수 체크
        if (currentTurnCount >= maxTurnCount)
        {
            Debug.LogWarning($"최대 턴 수({maxTurnCount}) 도달. 게임을 종료합니다.");
            OnMaxTurnReached?.Invoke();
            yield break;
        }

        currentTurnCount++;

        string turnOwner = playerTurn ? "플레이어" : "적";
        Debug.Log($"<color=cyan>턴 {currentTurnCount} 시작 ({turnOwner} 차례)</color>");

        // TODO: 턴 시작 UI 애니메이션
        yield return new WaitForSeconds(turnStartDelay);

        // 로딩 종료
        isLoading = false;

        // 실제 턴 시작
        if (playerTurn)
            StartPlayerTurnInternal();
        else
            StartEnemyTurnInternal();
    }

    // 플레이어 턴 시작
    void StartPlayerTurnInternal()
    {
        currentTurnState = TurnState.PlayerTurn;
        Debug.Log("<color=green>플레이어 턴 시작 (행동 가능)</color>");
        OnTurnStarted?.Invoke(true);
    }

    // 적 턴 시작
    void StartEnemyTurnInternal()
    {
        currentTurnState = TurnState.EnemyTurn;
        Debug.Log("<color=red>적 턴 시작 (행동 가능)</color>");
        OnTurnStarted?.Invoke(false);

        // TODO: 적 AI 행동 시작
        // 예: StartCoroutine(EnemyAI.TakeTurn());
    }

    // 턴 종료 요청
    public void EndTurn()
    {
        // 이미 로딩/전환 중이면 중복 EndTurn 방지
        if (isLoading)
        {
            Debug.LogWarning("TurnManager: 턴 전환 중에는 턴을 종료할 수 없습니다.");
            return;
        }

        // 턴이 진행 중인지 확인
        if (currentTurnState != TurnState.PlayerTurn && currentTurnState != TurnState.EnemyTurn)
        {
            Debug.LogWarning("TurnManager: 진행 중인 턴이 없습니다.");
            return;
        }

        StartCoroutine(EndTurnRoutine());
    }

    // 턴 종료 코루틴
    IEnumerator EndTurnRoutine()
    {
        isLoading = true;
        
        bool wasPlayerTurn = playerTurn;
        string turnOwner = wasPlayerTurn ? "플레이어" : "적";
        
        Debug.Log($"<color=yellow>{turnOwner} 턴 종료</color>");
        
        currentTurnState = TurnState.Ended;
        OnTurnEnded?.Invoke(wasPlayerTurn);

        // 턴 종료 연출
        yield return new WaitForSeconds(turnEndDelay);

        // 턴 전환
        playerTurn = !playerTurn;

        // 다음 턴 시작 코루틴 실행
        yield return StartCoroutine(TurnStartRoutine());
    }

    // 게임 강제 종료
    public void EndGame()
    {
        StopAllCoroutines();
        isLoading = false;
        currentTurnState = TurnState.Waiting;
        Debug.Log("<color=orange>게임 종료</color>");
    }

    // 현재 플레이어 턴인지 확인
    public bool IsPlayerTurn()
    {
        return playerTurn && currentTurnState == TurnState.PlayerTurn && !isLoading;
    }

    // 현재 적 턴인지 확인
    public bool IsEnemyTurn()
    {
        return !playerTurn && currentTurnState == TurnState.EnemyTurn && !isLoading;
    }

    // 턴 진행 가능 여부 확인 (행동 가능한지)
    public bool CanAct()
    {
        return !isLoading && (currentTurnState == TurnState.PlayerTurn || currentTurnState == TurnState.EnemyTurn);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"<b>Turn Manager Debug</b>");
        GUILayout.Label($"현재 턴: {currentTurnCount}");
        GUILayout.Label($"턴 상태: {currentTurnState}");
        GUILayout.Label($"플레이어 턴: {playerTurn}");
        GUILayout.Label($"로딩 중: {isLoading}");
        GUILayout.Label($"행동 가능: {CanAct()}");
        GUILayout.EndArea();
    }
#endif
}
