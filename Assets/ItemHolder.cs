using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour, IDropHandler
{

    [SerializeReference] public string ItemType;
    [SerializeReference] public int SlotType = 0;

    public ItemInstance item = null;

    private Image showImage;

    public void OnDrop(PointerEventData eventData)
    {
        if (DragPayload.ItemData != null)
        {
            if (DragPayload.dragType == ItemType)
            {
                if (ItemType == "Armor")
                {
                    if (DragPayload.ItemData.itemRef.wearSlot != SlotType)
                    {
                        DragPayload.ItemData = null;
                        DragPayload.icon = null;
                        DragPayload.dragType = "";
                        return;
                    }
                }

                if (item != null)
                {
                    if (item.holder != null)
                    {
                        item.holder.GetComponent<ItemHolder>().Unequip(false);
                    }
                }

                item = DragPayload.ItemData;

                if (item != null)
                {
                    if (item.holder != null)
                    {
                        item.holder.GetComponent<ItemHolder>().Unequip(false);
                    }
                }

                item.holder = transform;

                showImage.sprite = item.itemRef.icon;
                showImage.enabled = true;

                if (item.itemRef.itemType == "Armor")
                {
                    PlayerUI.UI.GetComponent<PlayerUI>().ToggleArmor(this);
                }
                else
                {
                    PlayerUI.UI.GetComponent<PlayerUI>().UpdateLoadOut();
                }
            }

        } else
        {
            showImage.enabled = false;
        }

        DragPayload.ItemData = null;
        DragPayload.icon = null;
        DragPayload.dragType = "";
    }

    public void Unequip(bool update)
    {
        if (item != null)
        {
            item.holder = null;

            showImage.enabled = false;

            item.storage.GetComponent<ItemInstanceButton>().UpdateUI();

            bool canUpdate = (item.itemRef.itemType != "Armor");

            item = null;

            if (update)
            {
                if (canUpdate)
                {
                    PlayerUI.UI.GetComponent<PlayerUI>().UpdateLoadOut();
                } else
                {
                    PlayerUI.UI.GetComponent<PlayerUI>().ToggleArmor(this);
                }
            }
        }
    }

    public void Select()
    {
        if (item != null && item.holder == transform)
        {
            SelectedItem.ItemData = item;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        showImage = transform.Find("Icon").transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
