using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject UI_Name;
    [SerializeField] GameObject UI_Des;
    [SerializeField] GameObject UI_Icon;
    [SerializeField] GameObject UI_Equip;
    [SerializeField] GameObject UI_Button;

    void Start()
    {
        
    }

    private bool equiped = false;

    // Update is called once per frame
    void Update()
    {
        if (SelectedItem.ItemData != null)
        {
            Image icon = UI_Icon.GetComponent<Image>();
            icon.sprite = SelectedItem.ItemData.itemRef.icon;
            UI_Icon.SetActive(true);

            TextMeshProUGUI name = UI_Name.GetComponent<TextMeshProUGUI>();
            name.SetText(SelectedItem.ItemData.itemRef.itemName);

            TextMeshProUGUI des = UI_Des.GetComponent<TextMeshProUGUI>();
            des.SetText(SelectedItem.ItemData.itemRef.itemDescription);

            if (SelectedItem.ItemData.holder != null) equiped = true;
            else equiped = false;

            TextMeshProUGUI equip = UI_Equip.GetComponent<TextMeshProUGUI>();
            if (equiped)
            {
                UI_Button.SetActive(true);
                equip.SetText("Unequip");
            }
            else
            {
                if (SelectedItem.ItemData.itemRef.isConsumable)
                {
                    UI_Button.SetActive(true);
                    equip.SetText("Use");
                }
                else
                {
                    UI_Button.SetActive(false);
                }
            }
        } else
        {
            UI_Icon.SetActive(false);
        }
    }

    public void Toggle()
    {
        if (SelectedItem.ItemData != null)
        {
            Transform holder = SelectedItem.ItemData.holder;
            if (holder != null && SelectedItem.action == "Unequip")
            {
                holder.GetComponent<ItemHolder>().Unequip(true);
            } else
            {

            }
        }
    }
}
