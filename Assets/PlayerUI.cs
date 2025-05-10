using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
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

    [SerializeField] GameObject ItemStats_UI;

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

    void Start()
    {
        Game.localPlayer = Current_Player;

        UI = transform;

        health = Current_Player.GetComponent<HealthSystem>();
        player = Current_Player.GetComponent<Player>();

        Inventory = Current_Player.GetComponent<PlayerInventory>();

        Loadout_UI = transform.parent.Find("Loadout").gameObject;

        ItemStats_UI = transform.parent.Find("ItemStats").gameObject;

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

            TextMeshProUGUI reserve = ItemStats_UI.transform.Find("Amount").Find("Text").GetComponent<TextMeshProUGUI>();
            if (usingWP.itemRef.canShoot)
            {
                reserve.SetText($"{usingWP.ammo}/{usingWP.itemRef.clipSize}");
            }
            else
            {
                reserve.SetText("N/A");
            }


            ItemStats_UI.SetActive(true);
        } else
        {
            ItemStats_UI.SetActive(false);
        }
    }

    // Inventory

    void LoadInventory()
    {
        foreach (var item in Inventory.Items)
        {
            if (item.storage == null)
            {
                AddInventoryButton(item);
            }
        }
    }

    public void AddInventoryButton(ItemInstance item)
    {
        if (item.storage == null)
        {
            item.storage = Instantiate(Button.transform, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Storage.transform);

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
