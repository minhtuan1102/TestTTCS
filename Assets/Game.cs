using NUnit.Framework;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public static GameObject g_enemies;
    public static GameObject g_items;
    public static GameObject g_players;
    public static GameObject g_projectiles;

    public static Item[] items;

    public static EnemyData[] enemies;

    public static GameObject ItemObjectSample;
    public static GameObject AreaAtkSample;

    public static GameObject prefab;

    public static GameObject localPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            g_enemies = GameObject.Find("Enemies");
            g_items = GameObject.Find("Items");
            g_players = GameObject.Find("Players");
            g_projectiles = GameObject.Find("Projectiles");

            ItemObjectSample = Resources.Load<GameObject>("Add/ItemObject");
            AreaAtkSample = Resources.Load<GameObject>("Add/AreaAttack");

            Instance = this;

            Game.items = Resources.LoadAll<Item>("Add/Item");
            Game.enemies = Resources.LoadAll<EnemyData>("Add/Enemy");

            foreach (var obj in Game.enemies)
            {
                obj.ID = obj.name;
                obj.path = "Add/EnemyModel/" + obj.EnemyModel.name;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public static EnemyData GetEnemyData(string name)
    {
        foreach (var obj in Game.enemies)
        {
            if (name == obj.name)
            {
                return obj;
            }
        }

        return Game.enemies[0];
    } 

    public static Item GetItemDataFromName(string name)
    {
        Item itemRef = null;
        foreach(Item item in items)
        {
            if (item.itemID == name)
            {
                itemRef = item;
                break;
            }
        }
        return itemRef;
    }

    public static int ToINT(string text)
    {
        int ans = 1;

        if (int.TryParse(text, out int result))
        {
            ans = result;
        }

        return ans;
    }
}
