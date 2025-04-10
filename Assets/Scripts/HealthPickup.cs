using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();

            // Chỉ nhặt khi máu chưa đầy
            if (health != null && health.CurrentHealth < health.MaxHealth)
            {
                health.Heal(healAmount);

                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }
}