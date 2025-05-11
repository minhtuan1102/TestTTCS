using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.Progress;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] public static Transform UI;
    [SerializeField] GameObject Current_Player;

    [Header("UI Bar")]

    [SerializeField] GameObject HP_UI;
    [SerializeField] GameObject MP_UI;
    [SerializeField] GameObject AP_UI;

    [Header("UI Display")]

    [SerializeField] GameObject Cash_UI;
    [SerializeField] GameObject Selected_UI; 

    [Header("UI Inventory")]

    [SerializeField] GameObject Storage;
    [SerializeField] GameObject Button;

    [SerializeField] GameObject Loadout_UI;
    [SerializeField] GameObject Iventory_UI;
    [SerializeField] GameObject ItemStats_UI;
    [SerializeField] GameObject Admin_UI;

    [SerializeField] public List<GameObject> Weapon_Slot = new List<GameObject>();
    [SerializeField] public List<GameObject> Consumer_Slot = new List<GameObject>();
    [SerializeField] public List<GameObject> Armor_Slot = new List<GameObject>();

    [SerializeField] public List<GameObject> Loadout_Consumer = new List<GameObject>();

    [SerializeField] private PlayerInventory Inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static bool useMainWP = true;

    private GameObject wp_loadout;
    private ItemInstance usingWP = null;

    HealthSystem health;
    Player player;

    // Item Interaction

    private float timer = 0f;
    private float atkCooldown = 0f;

    void Start()
    {
        Game.localPlayer = Current_Player;

        UI = transform;

        health = Current_Player.GetComponent<HealthSystem>();
        player = Current_Player.GetComponent<Player>();

        Inventory = Current_Player.GetComponent<PlayerInventory>();

        Loadout_UI = transform.parent.Find("Loadout").gameObject;

        ItemStats_UI = transform.parent.Find("ItemStats").gameObject;

        Admin_UI = transform.parent.Find("AdminPanel").gameObject;

        wp_loadout = Loadout_UI.transform.Find("Weapon").Find("Icon").gameObject;
        LoadInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP_UI != null)
        {
            Transform bar = HP_UI.transform.Find("Hider");
            RectTransform hider = bar.GetComponent<RectTransform>();
            float scale = Mathf.Clamp01((float)health.CurrentHealth / (float)health.MaxHealth);
            hider.localScale = new Vector3(1f - scale, 1, 1);

            Transform display = HP_UI.transform.Find("Text");
            TMP_Text text = display.GetComponent<TMP_Text>();
            text.text = $"{health.CurrentHealth}/{health.MaxHealth}";
        }

        if (AP_UI != null)
        {
            Transform bar = AP_UI.transform.Find("Hider");
            RectTransform hider = bar.GetComponent<RectTransform>();
            float scale = Mathf.Clamp01((float)health.CurrentArmor / (float)health.MaxArmor);
            hider.localScale = new Vector3(1f - scale, 1, 1);

            Transform display = AP_UI.transform.Find("Text");
            TMP_Text text = display.GetComponent<TMP_Text>();
            text.text = $"{health.CurrentArmor}/{health.MaxArmor}";
        }

        if (MP_UI != null)
        {
            Transform bar = MP_UI.transform.Find("Hider");
            RectTransform hider = bar.GetComponent<RectTransform>();
            float scale = Mathf.Clamp01((float)player.CurrentMana / (float)player.MaxMana);
            hider.localScale = new Vector3(1f - scale, 1, 1);

            Transform display = MP_UI.transform.Find("Text");
            TMP_Text text = display.GetComponent<TMP_Text>();
            text.text = $"{player.CurrentMana}/{player.MaxMana}";
        }

        if (Cash_UI != null)
        {
            TMP_Text text = Cash_UI.GetComponent<TMP_Text>();
            text.text = "$" + player.cash;
        }

        if (usingWP != null && usingWP.itemRef)
        {
            UnityEngine.UI.Image Icon = ItemStats_UI.transform.Find("Icon").Find("Image").GetComponent<UnityEngine.UI.Image>();
            Icon.sprite = usingWP.itemRef.icon;

            TextMeshProUGUI itemName = ItemStats_UI.transform.Find("Name").Find("Text").GetComponent<TextMeshProUGUI>();
            itemName.SetText(usingWP.itemRef.itemName);

            RectTransform scale = ItemStats_UI.transform.Find("Amount").Find("Scale").GetComponent<RectTransform>();

            TextMeshProUGUI reserve = ItemStats_UI.transform.Find("Amount").Find("Text").GetComponent<TextMeshProUGUI>();

            if (usingWP.itemRef.canShoot)
            {
                reserve.SetText($"{usingWP.ammo}/{usingWP.itemRef.clipSize}");
                scale.localScale = new Vector3(Mathf.Lerp(scale.localScale.x, (float)usingWP.ammo/(float)usingWP.itemRef.clipSize, Time.deltaTime * 10), 1f, 1f);
            }
            else
            {
                scale.localScale = new Vector3(1f, 1f, 1f);
                reserve.SetText("N/A");
            }


            ItemStats_UI.SetActive(true);
        }
        else
        {
            ItemStats_UI.SetActive(false);
        }

        if (SelectedItem.ItemData != null && SelectedItem.ItemData.itemRef)
        {
            Selected_UI.SetActive(true);
        }
        else
        {
            Selected_UI.SetActive(false);
        }

        timer += Time.deltaTime;
        atkCooldown -= Time.deltaTime;

        if (usingWP != null && usingWP.itemRef)
        {
            if (Input.GetMouseButton(0) && !Iventory_UI.activeSelf && !Admin_UI.activeSelf)
            {
                if (atkCooldown <= 0f)
                {
                    atkCooldown = usingWP.itemRef.cooldown;
                    Inventory.Attack();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItem(Consumer_Slot[0].GetComponent<ItemHolder>());
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseItem(Consumer_Slot[1].GetComponent<ItemHolder>());
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseItem(Consumer_Slot[2].GetComponent<ItemHolder>());
        }
    }
    // Inventory

    public void DropItem(int amount)
    {
        if (SelectedItem.ItemData != null && SelectedItem.ItemData.itemRef)
        {
            bool removed = false;
            Transform holder = SelectedItem.ItemData.holder;
            if (SelectedItem.ItemData.amount - amount <= 0)
            {
                if (holder != null)
                {
                    ItemHolder itemholder = holder.GetComponent<ItemHolder>();
                    itemholder.Unequip(true);
                }
                removed = true;
            }
            Inventory.DropItem(SelectedItem.ItemData, amount);
            UpdateInventory();

            if (removed)
            {
                SelectedItem.ItemData = null;
            }
        }
    }

    public void ToggleInventory()
    {
        Iventory_UI.SetActive(!Iventory_UI.activeSelf);
        if (Iventory_UI.activeSelf)
        {
            Admin_UI.SetActive(false);
        }
    }

    public void UpdateInventory()
    {
        LoadInventory();
    }

    public void UseSelectItem()
    {
        if (SelectedItem.ItemData != null && SelectedItem.ItemData.itemRef)
        {
            bool ranOut = false;
            Transform holder = SelectedItem.ItemData.holder;
            if (SelectedItem.ItemData.amount <= 1)
            {
                ranOut = true;
            }
            Inventory.UseItem(SelectedItem.ItemData);
            if (ranOut)
            {
               if (holder != null) holder.GetComponent<ItemHolder>().Unequip(true);
            }
        }
    }

    public void UseItem(ItemHolder holder)
    {
        if (holder.item != null && holder.item.itemRef)
        {
            bool ranOut = false;
            if (holder.item.amount <= 1)
            {
                ranOut = true;
            }
            Inventory.UseItem(holder.item);

            if (ranOut) holder.Unequip(true);
        }
    }

    void LoadInventory()
    {
        foreach (Transform item in Storage.transform)
        {
            item.gameObject.SetActive(false);
        }

        for (int index = 0; index < Inventory.Items.Count; index++)
        {
            ItemInstance item = Inventory.Items[index];

            Transform storageItem = null;
            if (index < Storage.transform.childCount)
            {
                storageItem = Storage.transform.GetChild(index);
            }
            if (storageItem == null)
            {
                AddInventoryButton(item);
            }
            else
            {
                item.storage = storageItem;
            }

            ItemInstanceButton itemButton = item.storage.GetComponent<ItemInstanceButton>();
            itemButton.item = item;
            itemButton.itemReference = item.itemRef;

            item.storage.gameObject.SetActive(true);
        }

        UpdateLoadOut();
    }

    public void AddInventoryButton(ItemInstance item)
    {
        if (item.storage == null)
        {
            item.storage = Instantiate(Button.transform, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Storage.transform);
            item.storage.gameObject.transform.SetAsLastSibling();

            ItemInstanceButton itemButton = item.storage.GetComponent<ItemInstanceButton>();
            itemButton.item = item;
            itemButton.itemReference = item.itemRef;

            item.storage.gameObject.SetActive(true);
        }
    }

    public void UpdateWP()
    {
        try
        {
            ItemHolder wp = null;
            if (useMainWP) wp = Weapon_Slot[0].GetComponent<ItemHolder>();
            else wp = Weapon_Slot[1].GetComponent<ItemHolder>();

            Inventory.holdingItem = wp.item;

            if (wp.item != null && wp.item.itemRef)
            {
                usingWP = wp.item;
                wp_loadout.transform.GetComponent<UnityEngine.UI.Image>().sprite = wp.item.itemRef.icon;
                wp_loadout.SetActive(true);
            }
            else
            {
                usingWP = null;
                wp_loadout.SetActive(false);
            }
        }
        catch
        {

        }
    }

    public void UpdateLoadOut()
    {
        for (int i=0; i<Consumer_Slot.Count; i++)
        {
            try
            {
                ItemHolder dat = Consumer_Slot[i].GetComponent<ItemHolder>();
                Transform loadout = Loadout_Consumer[i].transform;
                GameObject icon = loadout.Find("Icon").gameObject;
                GameObject amount = loadout.Find("Amount").gameObject;
                if (dat.item == null)
                {
                    icon.SetActive(false);
                    amount.SetActive(false);
                }
                else
                {
                    icon.transform.GetComponent<UnityEngine.UI.Image>().sprite = dat.item.itemRef.icon;
                    amount.transform.GetComponent<TextMeshProUGUI>().SetText("x"+dat.item.amount);
                    icon.SetActive(true);
                    amount.SetActive(true);
                }
            } catch
            {

            }
        }

        UpdateWP();
    }

}
