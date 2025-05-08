using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour, IDropHandler
{
    [SerializeReference] public string ItemType;

    public void OnDrop(PointerEventData eventData)
    {
        Image showImage = transform.Find("Icon").transform.GetComponent<Image>();
        
        if (DragPayload.icon != null)
        {
            if (DragPayload.dragType == ItemType)
            {
                showImage.sprite = DragPayload.icon;

                showImage.enabled = true;
            }

        } else
        {
            showImage.enabled = false;
        }

        DragPayload.icon = null;
        DragPayload.dragType = "";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
