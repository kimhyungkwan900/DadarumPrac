using UnityEngine;

public class PlayerInventory
{
    private readonly SaveManager _save;

    public PlayerInventory(SaveManager save){
        _save = save;
    }

    private static string Key(string itemKey) => $"item_{itemKey}";

    public int Count (string itemKey) => _save.GetVar(Key(itemKey));

    public bool Has(string itemKey, int amount = 1) => Count(itemKey) >= amount;

    public void Add(string itemKey, int amount =1){
        if (amount <= 0) return;
        _save.SetVar(Key(itemKey), Count(itemKey) + amount);
    }

    public bool TryConsume(string itemKey, int amount = 1){
        if (!Has(itemKey, amount)) return false;
        _save.SetVar(Key(itemKey), Count(itemKey) - amount);
        return true;
    }
}
