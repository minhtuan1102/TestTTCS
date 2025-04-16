using System;
using System.Collections;
using UnityEngine;

public class EnemySwing : MonoBehaviour
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
            attackCollider = hitbox.GetComponent<BoxCollider2D>();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Hitbox!");
        }

        enemyLayer = (1 << LayerMask.NameToLayer("Player"));
    }

    public void TriggerAttack(float damage)
    {
        StartCoroutine(DoAttack(damage));
    }

    private IEnumerator DoAttack(float damage)
    {
        // Bật collider
        attackCollider.enabled = true;

        // Chờ 1 frame hoặc vài milliseconds
        yield return new WaitForSeconds(0.05f);

        // Gây damage đúng 1 lần
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[5];
        int hitCount = attackCollider.Overlap(filter, results);

        for (int i = 0; i < hitCount; i++)
        {
            Player enemy = results[i].GetComponent<Player>();
            if (enemy != null)
            {
                HealthSystem hp = results[i].GetComponent<HealthSystem>();
                if (hp != null)
                {
                    hp.TakeDamage((int)damage);
                }
                break; // Chỉ đánh 1 địch
            }
        }

        // Tắt collider để không tiếp tục gây damage
        attackCollider.enabled = false;
    }
}
