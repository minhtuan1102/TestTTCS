using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviourPun
{
    private Rigidbody2D rb;
    [SerializeField] public ProjectileItem itemData;
    public float damage = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * itemData.speed;
        if (photonView.IsMine)
        {
            Invoke("DestroyProjectile", itemData.lifeTime);
        }
    }

    void DestroyProjectile()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Trúng địch!");
            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)damage);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        if (other.CompareTag("Barrier"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}