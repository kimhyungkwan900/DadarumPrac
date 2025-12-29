using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string currentScene;
    public Vector3 playerPosition;

    public Dictionary<string, bool> flags = new();
    public Dictionary<string, int> variables = new();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager I { get; private set; }
    public SaveData Data { get; private set; } = new SaveData();

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NewGame(){
        Data.flags.Clear();
        Data.variables.Clear();
        // TODO: 게임 초기 설정
    }

    public bool GetFlag(string key) =>
        Data.flags.TryGetValue(key, out var v) && v;

    public void SetFlag(string key, bool value = true) =>
        Data.flags[key] = value;

    public bool HasFlag(string key)
    {
        return Data.flags.ContainsKey(key);
    }

    public int GetVar(string key) =>
        Data.variables.TryGetValue(key, out var v) ? v : 0;

    public void SetVar(string key, int value){
        Data.variables[key] = value;
        Debug.Log($"SetVar: {key} = {value}");
    }
    public void AddVar(string key, int delta)
    {
        SetVar(key, GetVar(key) + delta);
    }
}
