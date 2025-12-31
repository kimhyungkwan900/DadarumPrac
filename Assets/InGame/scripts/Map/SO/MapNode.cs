using UnityEngine;

// 맵의 한 지점(노드)을 나타내는 ScriptableObject
[CreateAssetMenu(menuName = "Map/Map Node")]
public class MapNode : ScriptableObject
{
    [Header("노드 정보")]
    // 맵의 고유 ID
    public string mapId;
    
    // UI에 표시될 이름
    public string displayName;

    [Header("연결 정보")]
    [Tooltip("이 맵에서 이동 가능한 목적지 노드 목록")]
    public MapNode[] connectedMaps;
}