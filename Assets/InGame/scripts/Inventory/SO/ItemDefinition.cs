using UnityEngine;

public enum ItemMainType{
    Evidence,   // 증거
    Material,   // 재료
    Equipment,  // 장비
    Consumable, // 소모품
    Quest,      // 퀘스트
    Other,      // 기타
}

public enum ItemTags{
    None = 0,
    Evidence = 1 << 0,
    Material = 1 << 1,
    Equipment = 1 << 2,
    Crafting = 1 << 3,
    Quest = 1 << 4,
    Other = 1 << 5,
    All = Evidence | Material | Equipment | Crafting | Quest | Other,
}


public class ItemDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    public ItemMainType mainType;
    public ItemTags tags;
    public Sprite icon;
    public string description;
    public int value;
    public int stack;
    public int maxStack;
}
