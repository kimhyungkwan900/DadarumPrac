using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Cinemachine;

public class SceneFlowService : MonoBehaviour
{
    #region 컴포넌트
    [Header("Optional UX")]
    [SerializeField] private GameObject loadingUI; // 로딩 UI
    #endregion

    #region 필드
    public static SceneFlowService Instance { get; private set; }

    // 씬 전환 중 여부
    private bool isLoading = false;

    // 씬 전환 후 이동할 노드 정보
    private MapNode pendingNode;
    private string pendingSpawnId;

    // 외부용 씬 이동 완료 이벤트
    public event Action<MapNode> OnSceneMoveComplete;

    public GameObject player;
    #endregion

    #region 초기화
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        player = FindPlayer();
        if (player != null)
            TryCameraTracking(player);
    }

    #endregion

    #region Public API
    // 씬 이동 요청 (MoveService에서 호출하도록 설계)
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
    // 씬 로드 완료 시 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        player = FindPlayer();
        if (player != null)
            TryCameraTracking(player);

        // 스폰 위치 처리
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

        // 현재 맵 노드 정보 반영
        if (MoveService.Instance != null)
        {
            MoveService.Instance.SetCurrentMapNode(pendingNode);
        }

        Finish();
    }

    // 씬 전환 절차 마무리
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

    #region 유틸
    private GameObject FindPlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            Debug.LogWarning("[SceneFlowService] Player를 찾지 못했습니다.");
        }
        return p;
    }

    private void TryCameraTracking(GameObject player)
    {

        var cineCam = FindFirstObjectByType<CinemachineCamera>();
        if (cineCam != null)
        {
            cineCam.Follow = player.transform;
            cineCam.LookAt = player.transform;
        }
    }
    #endregion
}