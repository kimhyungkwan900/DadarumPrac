using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private SaveManager _save;

    public InventorySystem(SaveManager save){_save = save;}

    public bool Has(string key, int amount =1) => _save.GetItemCount(key) >= amount;

    public void Add(string key, int amount =1){
        if (amount <= 0) return;
        _save.SetItemCount(key, _save.GetItemCount(key) + amount);
        // TODO: 아이템 추가 시 플래그 업데이트
    }

    public bool TryConsume(string key, int amount =1){
        if (!Has(key, amount)) return false;
        _save.SetItemCount(key, _save.GetItemCount(key) - amount);
        return true;
    }
}
