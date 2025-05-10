using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    // Basic Stats
    public int itemID;
    public int amount = 0;
    public Transform holder = null;
    public Transform storage = null;
    public Item itemRef;

    // Firearm
    public int ammo = 0;

    // Modify
    public List<string> attachments;

    public ItemInstance(int id, int ammo, List<string> attachments, Item itemRef)
    {
        this.itemID = id;
        this.ammo = ammo;
        this.attachments = attachments ?? new List<string>();
        this.itemRef = itemRef;
    }

    public ItemInstance(ItemInstance other)
    {
        this.itemID = other.itemID;
        this.amount = other.amount;
        this.holder = other.holder;
        this.storage = other.storage;
        this.itemRef = other.itemRef;
        this.ammo = other.ammo;

        this.attachments = new List<string>(other.attachments);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ItemInstance FromJson(string json)
    {
        return JsonUtility.FromJson<ItemInstance>(json);
    }

    public ItemInstance Clone()
    {
        return new ItemInstance(
            itemID,
            ammo,
            new List<string>(attachments),
            itemRef
        )
        {
            amount = this.amount,
            holder = this.holder,
            storage = this.storage
        };
    }
}
