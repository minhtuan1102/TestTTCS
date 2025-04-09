using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public bool flipSpriteBasedOnDirection = true; // Thêm tùy chọn flip sprite

    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Thêm SpriteRenderer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy component SpriteRenderer

        // Lock rotation in Rigidbody2D
        rb.freezeRotation = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError($"Không tìm thấy GameObject với tag '{playerTag}'");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Xử lý hướng quay chính xác
            if (flipSpriteBasedOnDirection)
            {
                // Flip sprite theo hướng di chuyển (chỉ khi có SpriteRenderer)
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = direction.x < 0;
                }
                else
                {
                    // Fallback: sử dụng localScale nếu không có SpriteRenderer
                    transform.localScale = new Vector3(
                        Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x),
                        transform.localScale.y,
                        transform.localScale.z);
                }
            }

            if (distanceToPlayer > attackRange)
            {
                // Di chuyển
                rb.linearVelocity = direction * moveSpeed;

                if (animator != null)
                {
                    animator.SetBool("IsMoving", true);
                    animator.SetBool("IsAttacking", false);
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;

                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                }

                if (Time.time - lastAttackTime > attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsAttacking", false);
            }
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            var playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}