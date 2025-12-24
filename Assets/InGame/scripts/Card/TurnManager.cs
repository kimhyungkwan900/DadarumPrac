using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("Develop")]
    [SerializeField][Tooltip("시작 턴 모드를 정합니다")] ETurnMode eTurnMode;

    [Header("Properties")]
    public bool playerTurn;
    public bool isLoading;

    [Header("Turn Delay")]
    [SerializeField] float turnStartDelay = 0.3f;  // 턴 시작 연출 시간
    [SerializeField] float turnEndDelay = 0.3f;  // 턴 종료 연출 시간

    private enum ETurnMode { Random, Player, Enemy }
    private enum TurnState { Waiting, PlayerTurn, EnemyTurn, Ended }
    private TurnState currentTurnState;

    public static event Action<bool> OnTurnStarted;

    void GameSetup()
    {
        switch (eTurnMode)
        {
            case ETurnMode.Random: playerTurn = Random.Range(0, 2) == 0; break;
            case ETurnMode.Player: playerTurn = true; break;
            case ETurnMode.Enemy: playerTurn = false; break;
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        isLoading = true;

        GameSetup();

        // 바로 첫 턴 시작 코루틴으로 진입
        yield return StartCoroutine(TurnStartRoutine());
    }

    // 턴 시작 코루틴
    IEnumerator TurnStartRoutine()
    {
        isLoading = true;

        if (playerTurn)
            Debug.Log("내 턴 시작 (연출 대기 중)");
        else
            Debug.Log("상대 턴 시작 (연출 대기 중)");

        // TODO: 턴 시작 UI
        yield return new WaitForSeconds(turnStartDelay);

        // 실제 턴 시작
        isLoading = false;

        if (playerTurn)
            StartPlayerTurnInternal();
        else
            StartEnemyTurnInternal();
    }

    void StartPlayerTurnInternal()
    {
        currentTurnState = TurnState.PlayerTurn;
        Debug.Log("내 턴 시작 (실제)");
        OnTurnStarted?.Invoke(true);
    }

    void StartEnemyTurnInternal()
    {
        currentTurnState = TurnState.EnemyTurn;
        Debug.Log("상대 턴 시작 (실제)");
        OnTurnStarted?.Invoke(false);
    }
    public void EndTurn()
    {
        // 이미 로딩/연출 중이면 중복 EndTurn 방지
        if (isLoading)
            return;

        StartCoroutine(EndTurnRoutine());
    }

    IEnumerator EndTurnRoutine()
    {
        isLoading = true;
        currentTurnState = TurnState.Ended;

        // 턴 종료 연
        yield return new WaitForSeconds(turnEndDelay);

        // 턴 전환
        playerTurn = !playerTurn;

        // 다음 턴 시작 코루틴 실행
        yield return StartCoroutine(TurnStartRoutine());
    }
}
