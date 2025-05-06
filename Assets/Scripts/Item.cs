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

[CreateAssetMenu]
public class Item : ScriptableObject
{

    
    public string itemName;      // Name
    public Sprite icon;      // Icon
    public int itemID;           // ID của vật phẩm
    public string itemDescription;   // Mô tả của vật phẩm

    public GameObject Prefab;

    // Storage Stats

    public float weight = 0f;
    public int value = 0;

    // Consuming Stats

    public bool isConsumable = false;
    public string effectDescription = "";

    public List<MonoScript> effectsModules = new List<MonoScript>();

    // Attack Stats

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
        public Transform hitbox;

        // Shooting

        public bool canShoot = false;
        public int fireAmount = 1;
        public float spread = 0f;

        public Transform projectile;

        // Other
        public List<MonoBehaviour> options;
}
