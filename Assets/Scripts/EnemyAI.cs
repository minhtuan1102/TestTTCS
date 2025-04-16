using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI : MonoBehaviour
{
    private UnityEngine.Vector3 targetPos;
    private Transform target;

    NavMeshAgent agent;
    public LayerMask obstacleMask;
    public EnemyData data;

    // Timer

    private float chaseTimer = 0f;
    private float detectTimer = 0f;
    private float attackTimer = 0f;
    private float waitingTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

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

    private void Update()
    {
        // Cập nhật tất cả timer
        detectTimer += Time.fixedDeltaTime;
        attackTimer += Time.fixedDeltaTime;
        chaseTimer += Time.fixedDeltaTime;
        waitingTimer += Time.fixedDeltaTime;

        // Nếu đang trong thời gian "chờ" sau tấn công thì không làm gì
        if (waitingTimer < 0f)
        {
            return;
        }

        // Logic phát hiện player
        if (detectTimer >= 0f)
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
        if (attackTimer >= data.Attack_Rate && target != null) // Thêm điều kiện cooldown cho tấn công
        {
            if (CheckIfNear(target.position, data.Range))
            {
                attackTimer = 0f; // Đặt lại thời gian tấn công (reset cooldown)
                waitingTimer = -data.WaitTime; // Delay sau khi đánh
                EnemySwing swing_Attack = transform.Find("Attack").GetComponent<EnemySwing>();
                if (swing_Attack != null)
                {
                    swing_Attack.TriggerAttack(data.Damage);
                }
                agent.SetDestination(transform.position);
            }
        }

        // Di chuyển đến target nếu không chờ
        if (waitingTimer >= 0f && target != null)
        {
            agent.SetDestination(targetPos);
        }

    }
}