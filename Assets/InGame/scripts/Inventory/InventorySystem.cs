using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private readonly SaveManager save;
    private readonly Dictionary<string, ItemDefinition> itemDB = new();

    public IEnumerable<ItemDefinition> GetItemByTags(ItemTags tag){
        return itemDB.Values.Where(item => item.tags.HasFlag(tag) && save.GetItemCount(item.id) > 0);
    }
}
