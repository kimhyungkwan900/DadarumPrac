using System;
using UnityEngine;

// 맵 이동을 처리하는 서비스
public class MoveService : MonoBehaviour
{
    public static MoveService Instance { get; private set; }

    [SerializeField] private MapDestinationDatabase destinationDatabase;
    [SerializeField] private Transform playerTransform;

    public MapNode CurrentMapNode { get; private set; }
    public MapDestinationDatabase DestinationDatabase => destinationDatabase;

    public event Action<MapNode> OnMoveComplete;

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

    private void Start()
    {
        RefreshPlayerTransform();
    }

    // 플레이어 Transform 갱신 (씬 전환 후 호출)
    private void RefreshPlayerTransform()
    {
        if (playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    public bool TryMoveTo(MapDestination destination)
    {
        if (destination == null || destination.node == null)
            return false;

        if (destination.moveRange == MapMoveRange.External &&
            !EnergyManager.Instance.CanConsume(destination.actionPointCost))
            return false;

        if (destination.moveRange == MapMoveRange.External)
            EnergyManager.Instance.Consume(destination.actionPointCost);

        switch (destination.moveType)
        {
            case MapMoveType.SameScene:
                return MoveSameScene(destination);

            case MapMoveType.OtherScene:
                return MoveOtherScene(destination);
        }

        return false;
    }

    private bool MoveSameScene(MapDestination destination)
    {
        RefreshPlayerTransform();

        if (playerTransform == null)
        {
            Debug.LogWarning("[MoveService] playerTransform이 설정되지 않았습니다.");
            return false;
        }

        if (!SpawnRegistry.Instance.TryGet(destination.spawnId, out var spawn))
        {
            Debug.LogWarning($"[MoveService] SpawnId를 찾을 수 없습니다: {destination.spawnId}");
            return false;
        }

        Vector3 oldPos = playerTransform.position;
        playerTransform.position = spawn.position;

        CameraWarpService.Instance?.WarpTarget(
            playerTransform,
            spawn.position - oldPos
        );

        CurrentMapNode = destination.node;
        OnMoveComplete?.Invoke(destination.node);
        return true;
    }



    private bool MoveOtherScene(MapDestination destination)
    {
        CurrentMapNode = destination.node;

        return SceneFlowService.Instance.MoveToScene(
            destination.targetSceneName,
            destination.targetSpawnId,
            destination.node
        );
    }

    public void SetCurrentMapNode(MapNode node)
    {
        CurrentMapNode = node;
    }
}
