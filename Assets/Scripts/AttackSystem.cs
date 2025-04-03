using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer; // Chỉ đánh kẻ địch

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) // Nhấn phím Space để tấn công
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
            if (enemy.gameObject != gameObject) // Đảm bảo không tự đánh chính mình
            {
                Debug.Log("Tấn công: " + enemy.gameObject.name);
                enemy.GetComponent<HealthSystem>().TakeDamage(damage);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
