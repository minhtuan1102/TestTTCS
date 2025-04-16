using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D), typeof(PhotonView), typeof(HealthSystem))]
public class EnemyAI_PUN : MonoBehaviourPun
{
    [SerializeField] private string playerTag = "Player";
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public bool flipSpriteBasedOnDirection = true;

    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private HealthSystem healthSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthSystem = GetComponent<HealthSystem>();

        if (!PhotonNetwork.IsMasterClient)
        {
            enabled = false;
            return;
        }

        rb.freezeRotation = true;
        FindNearestPlayer();
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (player == null)
        {
            FindNearestPlayer();
            if (player == null) return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (flipSpriteBasedOnDirection && spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
                photonView.RPC("SyncFlip", RpcTarget.All, spriteRenderer.flipX);
            }

            if (distanceToPlayer > attackRange)
            {
                rb.linearVelocity = direction * moveSpeed;
                photonView.RPC("SyncAnimation", RpcTarget.All, true, false);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                photonView.RPC("SyncAnimation", RpcTarget.All, false, false);

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
            photonView.RPC("SyncAnimation", RpcTarget.All, false, false);
        }
    }

    void Attack()
    {
        photonView.RPC("SyncAnimation", RpcTarget.All, false, true);

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerView != null && playerHealth != null)
            {
                playerView.RPC("TakeDamage", RpcTarget.All, attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        healthSystem.TakeDamage(damage);
        photonView.RPC("PlayHitAnimation", RpcTarget.All);
    }

    [PunRPC]
    void SyncFlip(bool flipX)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX;
        }
    }

    [PunRPC]
    void SyncAnimation(bool isMoving, bool isAttacking)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    [PunRPC]
    void PlayHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (var playerObj in players)
        {
            float distance = Vector2.Distance(transform.position, playerObj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = playerObj.transform;
            }
        }

        player = closestPlayer;
        if (player == null)
        {
            Debug.LogWarning("Không tìm thấy người chơi nào!");
        }
    }

    void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.EnemyDefeated(gameObject);
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