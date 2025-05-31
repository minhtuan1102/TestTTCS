using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileData
{
    // Basic Stats
    public float speed;
    public float damage;
    public float lifeTime = 2f;
    public float kb = 0f;
    public List<DamageEffect> effect;
    public ProjectileItem special;

    public ProjectileData(float speed, float damage, float kb, float lifeTime, List<DamageEffect> effect, ProjectileItem special)
    {
        this.speed = speed;
        this.damage = damage;
        this.kb = kb;
        this.lifeTime = lifeTime;
        this.effect = effect;
        this.special = special;
    }

    public ProjectileData(ProjectileData item)
    {
        this.speed = item.speed;
        this.damage = item.damage;
        this.kb = item.kb;
        this.lifeTime = item.lifeTime;
        this.effect = item.effect;
        this.special = item.special;
    }
}


public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] public ProjectileData itemData;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * itemData.speed; // bắn theo hướng mặt phải
        Destroy(gameObject, itemData.lifeTime);
    }

    public void Knockback(Rigidbody2D rb, float kb)
    {
        if (rb == null) return;
        Vector2 direction = transform.right.normalized;
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * kb, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Gây sát thương nếu cần
        if (other.CompareTag("Enemy"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();

            if (health != null)
            {
                if (health.CurrentHealth > 0f)
                {
                    health.TakeDamage((int)itemData.damage);
                    Knockback(other.GetComponent<Rigidbody2D>(), itemData.kb);

                    foreach (DamageEffect effect in itemData.effect)
                    {
                        health.addEffect(effect);
                    }

                    try
                    {
                        if (itemData.special.areaAttack)
                        {
                            GameManager.SummonAttackArea(
                             rb.position,
                             Quaternion.Euler(0, 0, 0),
                             new AreaInstance(
                                 itemData.special.damage,
                                 itemData.special.knockBack,
                                 itemData.special.effects,
                                 itemData.special.hitbox,
                                 Game.g_enemies.transform
                                 )
                            );
                        }
                    } catch
                    {

                    }

                    Destroy(gameObject);
                }
            }
        }

        // Chạm tường xoá đạn
        if (other.CompareTag("Barrier"))
        {
            Destroy(gameObject);
        }
    }
}