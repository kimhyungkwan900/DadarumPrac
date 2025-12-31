using System.Collections.Generic;
using UnityEngine;

public class SpawnRegistry : MonoBehaviour
{
    public static SpawnRegistry Instance { get; private set; }

    private readonly Dictionary<string, Transform> map = new();

    private void Awake()
    {
        Instance = this;
        map.Clear();

        foreach (var sp in FindObjectsOfType<SpawnPoint>(true))
        {
            if (!string.IsNullOrEmpty(sp.spawnId))
                map[sp.spawnId] = sp.transform;
        }
    }

    public bool TryGet(string spawnId, out Transform transform)
    {
        return map.TryGetValue(spawnId, out transform);
    }
}
