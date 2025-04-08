using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Player tấn công!");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject != gameObject)
            {
                Debug.Log($"Tấn công: {enemy.gameObject.name} với sát thương {damage}");
                HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(damage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}