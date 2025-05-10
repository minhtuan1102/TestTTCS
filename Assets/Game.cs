using NUnit.Framework;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static GameObject g_enemies;
    public static GameObject g_items;
    public static GameObject g_players;

    public static Item[] items;

    public static GameObject ItemObjectSample;

    public static GameObject prefab;

    public static GameObject localPlayer;

    private void Start()
    {
        g_enemies = GameObject.Find("Enemies");
        g_items = GameObject.Find("Items");
        g_players = GameObject.Find("Players");

        items = Resources.LoadAll<Item>("Add/Item");

        ItemObjectSample = Resources.Load<GameObject>("Add/ItemObject");
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
