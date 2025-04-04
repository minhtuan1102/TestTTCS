﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;      // Name
    public Sprite itemIcon;      // Icon
    public int itemID;           // ID của vật phẩm
    public string description;   // Mô tả của vật phẩm

    public float weight;         // Trọng lượng của vật phẩm

    public bool ranged = false;

    public float damage = 0f;
    public float cooldown = 0f;

    
}
