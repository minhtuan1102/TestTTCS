using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemPickup : MonoBehaviour
{

    [SerializeReference] private float time_between = 3f;
    private GameObject items_collection;
    private bool can_Picked_Up = true;
    private float timer = 0f;

    private Transform playerHolder;

    private void Start()
    {
        items_collection = GameObject.Find("Items");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (playerHolder == null)
            {
                if (can_Picked_Up) if (other.CompareTag("Player"))  // Kiểm tra xem có phải người chơi không
                    {
                        playerHolder = other.transform;
                        Transform player_Hand = other.transform.Find("Main");
                        if (player_Hand.transform.childCount == 0)
                        {
                            Debug.Log("Người chơi đã nhặt vật phẩm!");
                            transform.position = player_Hand.position;
                            transform.SetParent(player_Hand);
                            transform.localRotation = Quaternion.Euler(0, 0, 0);
                        }
                    }
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer>=0) can_Picked_Up = true;

        if (playerHolder != null)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Drop();
            }
        }
    }

    public void Drop()
    {
        can_Picked_Up = false;
        playerHolder = null;
        transform.SetParent(items_collection.transform);
        timer = -3;
    }
}