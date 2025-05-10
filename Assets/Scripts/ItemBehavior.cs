using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class ItemPickup : MonoBehaviour
{

    [SerializeReference] private float time_between = 3f;
    private GameObject items_collection;
    private bool can_Picked_Up = true;
    private float timer = 0f;

    private Transform playerHolder;
    private float fireCooldown = 0f;

    [SerializeReference] public Item itemData; 

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
                        Transform player_Hand = other.transform.Find("Main");
                        if (player_Hand.transform.childCount == 0)
                        {
                            playerHolder = other.transform;
                            Debug.Log("Người chơi đã nhặt vật phẩm!");
                            transform.position = player_Hand.position;
                            transform.SetParent(player_Hand);
                            Vector3 localScale = transform.localScale;
                            localScale.y = Mathf.Abs(localScale.y);
                            transform.localScale = localScale;
                            transform.localRotation = Quaternion.Euler(0, 0, 0);

                            Player playerScript = playerHolder.GetComponent<Player>();
                            playerScript.swingOffset = itemData.swingOffset;
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

        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0)) // Left mouse button (fire)
        {
            Vector2 fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            // Example: Normal shot with base recoil

            // Example: Charged shot with **double recoil**
            // TriggerRecoil(fireDirection, 2f);

            if (playerHolder != null)
            {

                if (fireCooldown <= 0f)
                {
                    Player playerScript = playerHolder.transform.GetComponent<Player>();

                    Melee Attack_Swing = GetComponent<Melee>();
                    if (Attack_Swing != null)
                    {
                        playerScript.TriggerRecoil(itemData.recoil);
                        playerScript.TriggerSwing(itemData.swing);

                        Attack_Swing.TriggerAttack(itemData.damage);
                    }

                    FireBullet Attack_Shoot = GetComponent<FireBullet>();
                    if (Attack_Shoot != null)
                    {
                        playerScript.TriggerRecoil(itemData.recoil);
                        playerScript.TriggerSwing(itemData.swing);

                        Attack_Shoot.Shoot(itemData.damage, itemData.spread, itemData.fireAmount);
                    }

                    fireCooldown = itemData.cooldown;
                }
           
            }
        } 
        
    }

    public void Drop()
    {
        Player playerScript = playerHolder.GetComponent<Player>();
        playerScript.swingOffset = 0f;

        can_Picked_Up = false;
        playerHolder = null;

        transform.SetParent(items_collection.transform);
        timer = -3;
    }
}