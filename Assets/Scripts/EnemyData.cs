using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LootPackage
{
    [SerializeField] public ItemInstance[] Items;
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Items/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] public string ID;
    [SerializeField] public int Point = 100;

    [SerializeField] public float Health = 80;
    [SerializeField] public float Speed = 5f;

    [SerializeField] public float Damage = 10f;
    [SerializeField] public float Range = 5f;
    [SerializeField] public float Attack_Rate = 1f;

    [SerializeField] public GameObject Attack_Hitbox;

    [SerializeField] public float WaitTime = 0.5f;

    [SerializeField] public GameObject EnemyModel;

    [SerializeField] public float Detection_Range = 10f;
    [SerializeField] public float Detection_Rate = 0.2f;

    [SerializeField] public float Chasing_Time_Threshold = 0.2f;

    [SerializeField] public string path = "";

    [SerializeField] public List<LootPackage> Loot = new List<LootPackage>();
}
