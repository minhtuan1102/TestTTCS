using TMPro;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemInstance Data;

    void OnEnable()
    {
        Transform model = transform.Find("Model");
        if (model.childCount == 0)
        {
            GameObject newModel = Instantiate(Data.itemRef.model, model);

            newModel.transform.localPosition = Vector3.zero;
        }
        transform.Find("Canvas").Find("Name").GetComponent<TextMeshProUGUI>().SetText($"x{Data.amount} {Data.itemRef.itemName}");
    }
}
