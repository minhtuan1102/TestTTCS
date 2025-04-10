using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] public ProjectileItem itemData;

    public float damage = 0f;


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
            Debug.Log("Trúng địch!");
            Enemy enemyData = other.GetComponent<Enemy>();

            if (enemyData != null)
            {
                if (enemyData.health > 0f)
                {
                    enemyData.TakeDamage(damage);
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
