using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemInstance Data;

    private void Start()
    {
        transform.GetComponent<SpriteRenderer>().sprite = Data.itemRef.icon;
    }
}
