using UnityEngine;
using System.Collections.Generic;

// 모든 MapDestination 에셋들을 관리하고 빠른 조회를 제공하는 데이터베이스
[CreateAssetMenu(menuName = "Map/Map Destination Database")]
public class MapDestinationDatabase : ScriptableObject
{
    [SerializeField]
    [Tooltip("데이터베이스에 포함될 모든 목적지 에셋")]
    private MapDestination[] destinations;

    // 빠른 조회를 위한 MapNode-MapDestination 딕셔너리
    private Dictionary<MapNode, MapDestination> map;

    // 에셋이 활성화될 때 딕셔너리 초기화
    private void OnEnable()
    {
        map = new Dictionary<MapNode, MapDestination>();
        foreach (var d in destinations)
        {
            if (d != null && d.node != null)
                map[d.node] = d;
        }
    }

    // 특정 맵 노드에 해당하는 목적지 정보를 조회
    public bool TryGet(MapNode node, out MapDestination dest)
    {
        // 맵이 초기화되지 않았을 경우를 대비
        if (map == null) 
        {
            OnEnable();
        }
        
        return map.TryGetValue(node, out dest);
    }
}