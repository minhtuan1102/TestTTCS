using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] public List<ItemInstance> Items;

    [SerializeField] public float pickUpRange = 2f;

    [SerializeField] public ItemInstance holdingItem;

    [SerializeField] public List<ItemInstance> Armor;

    private Transform nearestItem;

    private float scanTime = 0.2f;
    private float scanTimer = 0f;

    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
    }

    public void Holding(ItemInstance item)
    {
        if (Items.Contains(item))
        {
            holdingItem = item;
        }
    }

    // Game function

    public int FindItem(ItemInstance item)
    {
        int index = 0;
        for (index = 0; index < Items.Count; index++)
        {
            if (Items[index] != null)
            {
                if (item.itemRef.itemName == Items[index].itemRef.itemName)
                {
                    return index;
                }
            }
        }
        return -1;
    }

    public void AddItem(ItemInstance item)
    {
        int index = FindItem(item);
        if (index>=0 && item.itemRef.isConsumable)
        {
            Items[index].amount += item.amount;
        } else
        {
            Items.Add(new ItemInstance(item));
            PlayerUI.UI.GetComponent<PlayerUI>().AddInventoryButton(item);
        }
    }

    public void DropItem(ItemInstance item)
    {
        int index = Items.IndexOf(item);
    }

    public void UseItem(ItemInstance item)
    {
        int index = Items.IndexOf(item);
    }

    // Pick item

    private Transform FindNearItem()
    {
        Transform nearest = null;
        float nearestDis = pickUpRange;
        float dis;
        if (Game.items != null)
        {
            foreach (Transform item in Game.g_items.transform)
            {
                dis = (item.position - transform.position).sqrMagnitude;
                if (dis < pickUpRange)
                {
                    if (nearest == null || (dis < nearestDis)) 
                    {
                        nearest = item;
                        nearestDis = dis;
                    }
                }
            }
        }

        return nearest;
    }

    void Update()
    {
        scanTimer += Time.fixedDeltaTime;

        if (scanTimer > 0f)
        {
            scanTimer -= scanTime;

            if (nearestItem != null)
            {
                GameObject canvas = nearestItem.transform.Find("Canvas").gameObject;
                canvas.SetActive(false);
            }

            nearestItem = FindNearItem();
        }

        if (nearestItem != null)
        {
            GameObject canvas = nearestItem.transform.Find("Canvas").gameObject;
            canvas.SetActive(true);

            canvas.transform.localPosition = new Vector3(0, 0.6f + 0.1f*Mathf.Cos(Time.time), 0);

            if (Input.GetKeyDown(KeyCode.E))
            {
                AddItem(nearestItem.GetComponent<ItemObject>().Data);
                Destroy(nearestItem.gameObject);
                nearestItem = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerUI.useMainWP = !(PlayerUI.useMainWP);
            PlayerUI.UI.transform.GetComponent<PlayerUI>().UpdateWP(); 
        }

        if (holdingItem != null && holdingItem.itemRef)
        {
            player.HandItem.transform.GetComponent<SpriteRenderer>().sprite = holdingItem.itemRef.icon;
            player.HandItem.gameObject.SetActive(true);
        } else
        {
            player.HandItem.gameObject.SetActive(false);
        }
    }

}
