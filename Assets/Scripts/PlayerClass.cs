using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerClass", menuName = "Items/PlayerClass")]
public class PlayerClass : ScriptableObject
{
    [Header("Info")]
    public int id = 0;
    public string class_Name = "";
    public string des = "";

    [Space]

    [Header("Stats")]

    public float health = 100f;
    public float mana = 100f;
    public float armor = 0f;

    [Space]

    public float speed = 5f;

    [Space]

    [Header("Model")]

    public Sprite Head;
    public Sprite Hair;
    public Sprite Body;
    public Sprite Leg;

    [Space]

    [Header("Starter Inventory")]

    public List<ItemInstance> loadout;
}
