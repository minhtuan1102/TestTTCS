using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[System.Serializable]
public class ItemInstanceSender
{
    // Basic Stats
    public int itemID;
    public int amount = 0;
    public int holder = -1;
    public int storage = -1;
    public string itemRef = "";

    // Firearm
    public int ammo = 0;
    public bool reloading = false;
    // Modify
    public List<string> attachments;

    public ItemInstanceSender(int itemID, int amount, int holder, int storage, string itemRef, int ammo, bool reloading, List<string> attachments)
    {
        this.itemID = itemID;
        this.amount = amount;
        this.holder = holder;
        this.storage = storage;
        this.itemRef = itemRef;
        this.ammo = ammo;
        this.reloading = reloading;
        this.attachments = attachments;
    }

    public ItemInstanceSender(ItemInstance item)
    {
        this.itemID = item.itemID;
        this.amount = item.amount;
        this.holder = -1;

        if (item.holder != null) this.holder = PlayerUI.Holder.IndexOf(item.holder);

        this.storage = -1;

        if (item.holder != null) this.holder = PlayerUI.Holder.IndexOf(item.holder);

        this.itemRef = item.itemRef.itemID;
        this.ammo = item.ammo;
        this.reloading = item.reloading;
        this.attachments = item.attachments;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ItemInstanceSender FromJson(string json)
    {
        return JsonUtility.FromJson<ItemInstanceSender>(json);
    }
}

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
    public bool reloading = false;
    // Modify
    public List<string> attachments;

    public ItemInstance(ItemInstance other)
    {
        this.itemID = other.itemID;
        this.amount = other.amount;
        this.holder = other.holder;
        this.storage = other.storage;
        this.itemRef = other.itemRef;
        this.ammo = other.ammo;

        if (other.attachments != null) this.attachments = new List<string>(other.attachments);
    }

    public ItemInstance(ItemInstanceSender other)
    {
        this.itemID = other.itemID;
        this.amount = other.amount;
        this.holder = null;

        if (other.holder >= 0) this.holder = PlayerUI.Holder[other.holder];

        this.storage = null;

        if (other.holder >= 0) this.holder = PlayerUI.Holder[other.holder];

        this.itemRef = Game.GetItemDataFromName(other.itemRef);
        this.ammo = other.ammo;

        this.attachments = new List<string>(other.attachments);
    }

    public ItemInstance(int id, Item itemRef, int ammo, int amount)
    {
        this.itemID = id;
        this.amount = amount;
        this.ammo = ammo;
        this.itemRef = itemRef;
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
