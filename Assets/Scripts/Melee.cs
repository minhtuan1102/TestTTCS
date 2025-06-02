using System;
using UnityEngine;

public class Melee : MonoBehaviour
{

    [Header("Collider dùng để kiểm tra va chạm")]
    private Collider2D attackCollider;

    [Header("Layer của kẻ địch")]
    private LayerMask enemyLayer;

    private void Start()
    {
        Transform hitbox = transform.Find("Hitbox");
        if (hitbox != null)
        {
            attackCollider = hitbox.GetComponent<PolygonCollider2D>();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Hitbox!");
        }

        enemyLayer = (1 << LayerMask.NameToLayer("Enemy"));
    }

    public void TriggerAttack(float damage)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true; // Nếu collider là trigger

        Collider2D[] results = new Collider2D[10]; // Mảng kết quả
        int hitCount = attackCollider.Overlap(filter, results);

        for (int i = 0; i < hitCount; i++)
        {
            
            Enemy enemy = results[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                //enemy.TakeDamage(damage);
                Debug.Log("Enemy hit: " + enemy.name);
            }
        }
    }

}