using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AttackScript
{
    public MonoBehaviour Script;
    public Collider2D Collider;
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    QuestItem
}

[CreateAssetMenu]
public class Item : ScriptableObject
{

    
    public string itemName;
    public string itemType;
    public ItemType _itemType;
    public Sprite icon;
    public string itemID;          
    public string itemDescription;

    public GameObject Prefab;

    // Storage Stats

    public float weight = 0f;
    public int value = 0;

    // Consuming Stats

    public bool isConsumable = false;
    public string effectDescription = "";

    public List<MonoScript> effectsModules = new List<MonoScript>();

    // Attack Stats
    public bool isWeapon = false;
    public bool canAttack = false;

        // Animation

        public float swing = 0f;
        public float recoil = 0f;
        public float swingOffset = 0f;

        // Stats

        public float damage = 0f;
        public float cooldown = 0f;
        public float manaConsume = 0f;   

        // Melee
        public bool canMelee = false;
        public Transform hitbox;

        // Shooting

        public bool canShoot = false;
        public int fireAmount = 1;
        public int maxAmmo = 1;
        public float spread = 0f;

        public int clipSize = 0;
        public float reload = 0f;

        public Transform projectile;

        // Other
        public List<MonoBehaviour> options;
}
