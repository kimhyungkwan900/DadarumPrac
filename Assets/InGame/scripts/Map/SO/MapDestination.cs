using UnityEngine;

// 맵 이동 타입을 정의하는 열거형
public enum MapMoveType
{
    SameScene,  // 같은 씬 내에서 이동
    OtherScene  // 다른 씬으로 이동
}

// 이동 범위 타입 
public enum MapMoveRange
{
    External,   // 외부 이동 (에너지 필요)
    Internal    // 내부 이동 (에너지 불필요)
}

// 특정 맵 노드로 이동할 때의 상세 정보를 담는 ScriptableObject
[CreateAssetMenu(menuName = "Map/Map Destination")]
public class MapDestination : ScriptableObject
{
    [Header("기본 정보")]
    // 목적지 맵 노드
    public MapNode node;

    // 맵 이동 타입
    public MapMoveType moveType;

    // 이동 범위 타입 (외부/내부)
    public MapMoveRange moveRange = MapMoveRange.External;

    // 이동에 소모되는 행동 포인트 (내부 이동일 경우 무시됨)
    [Range(0, 3)]
    public int actionPointCost;

    [Header("Same Scean 이동정보")]
    public string spawnId;

    [Header("다른 씬(Other Scene) 이동 정보")]
    // 이동할 대상 씬의 이름
    public string targetSceneName;
    
    // 대상 씬에서 스폰될 지점의 고유 ID
    public string targetSpawnId;
}