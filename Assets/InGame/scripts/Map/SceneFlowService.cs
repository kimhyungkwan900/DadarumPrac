using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneFlowService : MonoBehaviour
{
    public static SceneFlowService Instance { get; private set; }

    [Header("Optional UX")]
    [SerializeField] private GameObject loadingUI; // 나중에 연결

    // 씬 전환 중 상태
    private bool isLoading = false;

    // 전달받은 이동 정보
    private MapNode pendingNode;
    private string pendingSpawnId;

    // 외부에서 구독 가능
    public event Action<MapNode> OnSceneMoveComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Public API

    // 씬 이동 요청 (MoveService만 호출하도록 권장)
    public bool MoveToScene(
        string sceneName,
        string spawnId,
        MapNode arrivedNode
    )
    {
        if (isLoading)
        {
            Debug.LogWarning("[SceneFlowService] 이미 씬 전환 중입니다.");
            return false;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneFlowService] sceneName이 비어 있습니다.");
            return false;
        }

        isLoading = true;
        pendingNode = arrivedNode;
        pendingSpawnId = spawnId;

        // UX 처리
        if (loadingUI != null)
            loadingUI.SetActive(true);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);

        return true;
    }

    #endregion

    #region Internal

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Player 찾기
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[SceneFlowService] Player를 찾지 못했습니다.");
            Finish();
            return;
        }

        // Spawn 처리 (SpawnRegistry 사용)
        if (!string.IsNullOrEmpty(pendingSpawnId))
        {
            if (SpawnRegistry.Instance == null)
            {
                Debug.LogWarning("[SceneFlowService] SpawnRegistry가 없습니다.");
            }
            else if (SpawnRegistry.Instance.TryGet(pendingSpawnId, out var spawn))
            {
                player.transform.position = spawn.position;
            }
            else
            {
                Debug.LogWarning($"[SceneFlowService] SpawnId를 찾지 못함: {pendingSpawnId}");
            }
        }

        // 현재 맵 상태 반영
        if (MoveService.Instance != null)
        {
            MoveService.Instance.SetCurrentMapNode(pendingNode);
        }

        Finish();
    }


    private void Finish()
    {
        isLoading = false;

        if (loadingUI != null)
            loadingUI.SetActive(false);

        OnSceneMoveComplete?.Invoke(pendingNode);

        pendingNode = null;
        pendingSpawnId = null;
    }

    #endregion
}
