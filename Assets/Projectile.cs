using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] public ProjectileItem itemData;

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
            other.GetComponent<Enemy>().TakeDamage(itemData.damage);
            Destroy(gameObject);
        }
    }
}
