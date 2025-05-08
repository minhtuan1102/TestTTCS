using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemInstanceButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("UI")]

    [SerializeReference] public Transform UI;

    [Header("Basic Info")]

    [SerializeReference] public string itemType;
    [SerializeReference] public int itemAmount = 0;
    [SerializeReference] public int itemAmmo = 0;
    [SerializeReference] public bool itemEquiped = false;

    [SerializeReference] private Item itemReference;

    GameObject dragger;

    void Start()
    {
        //UpdateUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject icon = transform.Find("Icon").transform.Find("Item").gameObject;

        dragger = Instantiate(icon, UI.transform);

        DragPayload.icon = icon.transform.GetComponent<Image>().sprite;
        DragPayload.dragType = itemReference.itemType;

        dragger.transform.GetComponent<Image>().raycastTarget = false;
        dragger.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragger)
        {
            dragger.transform.position = Vector3.Lerp(dragger.transform.position, Input.mousePosition, 10f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragger)
        {
            Destroy(dragger.transform.GetComponent<Image>());
            Destroy(dragger);
            dragger = null;
        }
        UpdateUI();
    }

    // Update is called once per frame
    void UpdateUI()
    {
        // Display Ammo
        GameObject ammoDisplay = transform.Find("Ammo").gameObject;
        if (itemReference.canShoot)
        {
            ammoDisplay.transform.GetComponent<TextMeshProUGUI>().SetText("Ammo:" + itemAmmo + "/" + itemReference.maxAmmo);
            ammoDisplay.SetActive(true);
        } else ammoDisplay.SetActive(false);

        // Display Name
        GameObject itemNameDisplay = transform.Find("Name").gameObject;
        itemNameDisplay.transform.GetComponent<TextMeshProUGUI>().SetText(itemReference.name);

        // Display Amount
        GameObject amountDisplay = transform.Find("Amount").gameObject;
        if (itemReference.itemType != "Weapon")
        {
            amountDisplay.transform.GetComponent<TextMeshProUGUI>().SetText("x" + itemAmount.ToString());
            amountDisplay.SetActive(true);
        } else amountDisplay.SetActive(false);

        // Display Equiped
        GameObject equipedDisplay = transform.Find("IsUsing").gameObject;
        if (itemEquiped) equipedDisplay.SetActive(true);
        else equipedDisplay.SetActive(false);

        // Display Type
        GameObject typeDisplay = transform.Find("Type").gameObject;
        typeDisplay.transform.GetComponent<TextMeshProUGUI>().SetText(itemReference.itemType);

        // Display Icon
        GameObject iconDisplay = transform.Find("Icon").transform.Find("Item").gameObject;
        iconDisplay.transform.GetComponent<Image>().sprite = itemReference.icon;
    }

    void Update()
    {
        
    }
}
