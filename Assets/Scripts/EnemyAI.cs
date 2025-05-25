using Photon.Pun;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI : MonoBehaviour
{
    private UnityEngine.Vector3 targetPos;
    private Transform target;

    private Rigidbody2D rb;

    NavMeshAgent agent;
    private NavMeshPath path;

    public LayerMask obstacleMask;
    private EnemyData data;

    private Enemy enemy;
    // Timer

    private float chaseTimer = 0f;
    private float detectTimer = 0f;
    private float attackTimer = 0f;
    private float waitingTimer = 0f;

    public UnityEngine.Vector3 MoveDirection;
    public float Speed { get; private set; }

    private int currentCorner = 0;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        path = new NavMeshPath();
    }

    void Start()
    {
        enemy = GetComponent<Enemy>();
        data = enemy.data;

        agent.speed = data.Speed;
    }

    private Transform FindPlayer()
    {
        Transform detectedPlayer = null;
        float minDistance = Mathf.Infinity;
        foreach (Transform player in GameObject.Find("Players").transform)
        {
            UnityEngine.Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = UnityEngine.Vector2.Distance(transform.position, player.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);

            if (hit.collider == null)
            {
                if (minDistance > distanceToPlayer)
                {
                    minDistance = distanceToPlayer;
                    detectedPlayer = player;
                }
            }
        }
        return detectedPlayer;
    }

    bool CheckIfNear(UnityEngine.Vector3 pos, float radius)
    {
        if (UnityEngine.Vector3.Distance(transform.position, pos) <= radius)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Cập nhật tất cả timer
        detectTimer += Time.fixedDeltaTime;
        attackTimer += Time.fixedDeltaTime;
        chaseTimer += Time.fixedDeltaTime;
        waitingTimer += Time.fixedDeltaTime;

        UnityEngine.Vector3 velocity = agent.velocity;

        Speed = velocity.magnitude;

        if (Speed > 0.01f)
        {
            MoveDirection = velocity.normalized;
        }
        else
        {
            MoveDirection = UnityEngine.Vector3.zero;
        }

        // Logic phát hiện player
        if (detectTimer >= 0f && waitingTimer >= 0f)
        {
            Transform detected = FindPlayer();
            if (detected != null && CheckIfNear(detected.position, data.Detection_Range))
            {
                target = detected;
                targetPos = detected.position;
                chaseTimer = -data.Chasing_Time_Threshold;
            }
            else
            {
                if (chaseTimer >= 0f)
                {
                    target = null;
                }
            }
            detectTimer = -data.Detection_Rate;
        }

        // Logic tấn công nếu đủ điều kiện
        if (attackTimer >= 0 && target != null) // Thêm điều kiện cooldown cho tấn công
        {
            if (CheckIfNear(target.position, data.Range))
            {
                attackTimer = -data.Attack_Rate; // Đặt lại thời gian tấn công (reset cooldown)
                waitingTimer = -data.WaitTime; // Delay sau khi đánh

                GameManager.SummonAttackArea(
                        transform.position,
                        UnityEngine.Quaternion.Euler(0, 0, enemy.lookDir),
                        new AreaInstance(data.Damage, data.Attack_Hitbox, Game.g_players.transform)
                        );

                /*
                EnemySwing swing_Attack = transform.Find("Attack").GetComponent<EnemySwing>();
                if (swing_Attack != null)
                {
                    swing_Attack.TriggerAttack(data.Damage);
                }
                agent.SetDestination(transform.position);
                */
            }
        }

        // Di chuyển đến target nếu không chờ
        if (waitingTimer >= 0f && target != null)
        {
            agent.SetDestination(targetPos);
        }


        enemy.lookAtPos = targetPos;

        UnityEngine.Vector2 direction = (agent.steeringTarget - (UnityEngine.Vector3)rb.position);
        rb.linearVelocity = direction.normalized * data.Speed;
    }
}