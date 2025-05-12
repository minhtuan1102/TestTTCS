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

    public ProjectileData(float speed, float damage, float lifeTime)
    {
        this.speed = speed;
        this.damage = damage;
        this.lifeTime = lifeTime;
    }

    public ProjectileData(ProjectileData item)
    {
        this.speed = item.speed;
        this.damage = item.damage;
        this.lifeTime = item.lifeTime;
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
                    if (PhotonNetwork.IsMasterClient)
                    {
                        health.TakeDamage((int)itemData.damage);
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