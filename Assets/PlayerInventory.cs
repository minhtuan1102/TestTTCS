using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    // Basic Stats
    public int itemID;
    public int amount = 0;

    // Firearm
    public int ammo = 0;

    // Modify
    public List<string> attachments;

    public ItemInstance(int id, int ammo, List<string> attachments)
    {
        this.itemID = id;
        this.ammo = ammo;
        this.attachments = attachments ?? new List<string>();
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ItemInstance FromJson(string json)
    {
        return JsonUtility.FromJson<ItemInstance>(json);
    }
}

public class PlayerInventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<ItemInstance> storage = new List<ItemInstance>();

    public void AddItem()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
