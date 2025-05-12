using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AttackScript
{
    public MonoBehaviour Script;
    public Collider2D Collider;
}

[System.Serializable]
public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    QuestItem
}

[System.Serializable]
public enum ValueType
{
    Int,
    Float,
    Bool,
    String
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

    public GameObject model;

    // Storage Stats

    public float weight = 0f;
    public int value = 0;

    // Consuming Stats

    public bool isConsumable = false;
    public string effectDescription = "";

    public List<Modify> consumeEffect = new List<Modify>();

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
        

        // Melee
        public bool canMelee = false;
        public GameObject hitbox;
        public float mele_manaConsume = 0f;
        // Shooting

        public bool canShoot = false;
        public int fireAmount = 1;
        public int maxAmmo = 1;
        public float spread = 0f;
        public float bulletSpeed = 5f;
        public float bulletLifetime = 5f;
        public float shooting_manaConsume = 0f;
        public float reload_manaConsume = 0f;

        public int clipSize = 0;
        public float reload = 0f;

        public GameObject projectile;

        // Other
        public List<MonoBehaviour> options;
}
