using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1f;
    public int damage = 10;
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (Time.time - lastAttackTime > attackCooldown)
        {
            Debug.Log("Enemy tấn công!");
            player.GetComponent<HealthSystem>().TakeDamage(damage);
            lastAttackTime = Time.time;
            Debug.Log("Tấn công: " + player.gameObject.name);
        }
    }
}
