using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    [SerializeField] private float staminaAmount = 30f;
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStamina stamina = other.GetComponent<PlayerStamina>();

            // Chỉ nhặt khi thể lực chưa đầy
            if (stamina != null && stamina.currentStamina < stamina.maxStamina)
            {
                stamina.RestoreStamina(staminaAmount);

                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }
}