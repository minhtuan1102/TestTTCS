﻿using Photon.Pun;
using System;
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
    private float ranged_AttackTimer = 0f;
    private float waitingTimer = 0f;

    private float waitingThresdhold = 0f;

    public UnityEngine.Vector3 MoveDirection;
    public float Speed { get; private set; }

    private int currentCorner = 0;
    private PhotonView photonView;

    public List<float> meleeAttacks = new List<float>();
    public List<float> rangedAttacks = new List<float>();

    private float stunTime = 0f;

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

        if (data.meleeAttacks.Count>0)
        {
            for (int i = 0; i < data.meleeAttacks.Count; i++)
            {
                meleeAttacks.Add(data.meleeAttacks[i].delay);
            }
        }

        if (data.rangedAttacks.Count>0)
        {
            for (int i = 0; i < data.rangedAttacks.Count; i++)
            {
                rangedAttacks.Add(data.rangedAttacks[i].delay);
            }
        }

        agent.speed = data.Speed;
    }

    private Transform FindPlayer(bool hunting)
    {
        Transform detectedPlayer = null;
        float minDistance = Mathf.Infinity;
        foreach (Transform player in GameObject.Find("Players").transform)
        {
            UnityEngine.Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = UnityEngine.Vector2.Distance(transform.position, player.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);

            if (hit.collider == null || hunting)
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

    public void Stunned(float duration)
    {
        if (duration > 0f)
        {
            stunTime = duration;
            agent.enabled = false;
        }
    }

    [PunRPC]
    private void RPC_RangedAttack(int i, float dir, UnityEngine.Vector3 pos)
    {
        EnemyRangedAttack enemyRangedAttack = data.rangedAttacks[i];
        GameManager.SummonProjectile(transform.gameObject,
            pos,
            UnityEngine.Quaternion.Euler(0, 0, dir),
            new ProjectileData(
                enemyRangedAttack.ranged_bulletSpeed,
                enemyRangedAttack.Damage,
                enemyRangedAttack.Attack_KB,
                enemyRangedAttack.Attack_KBDuration,
                enemyRangedAttack.ranged_bulletLifetime,
                enemyRangedAttack.Attack_Effect,
                enemyRangedAttack.ranged_projectileDat,
                Game.g_players.transform
            ),
            enemyRangedAttack.ranged_projectile
        );
    }

    [PunRPC]
    private void RPC_ToggleHidden(bool toggle)
    {
        if (toggle)
        {
            enemy.model.gameObject.SetActive(true);
            enemy.main_hand.SetActive(true);
        }
        else
        {
            enemy.model.gameObject.SetActive(false);
            enemy.main_hand.SetActive(false);
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Cập nhật tất cả timer
        waitingThresdhold += Time.fixedDeltaTime;
        detectTimer += Time.fixedDeltaTime;
        attackTimer += Time.fixedDeltaTime;

        for (int i=0; i<meleeAttacks.Count; i++)
        {
            meleeAttacks[i] += Time.fixedDeltaTime;
        }

        for (int i = 0; i < rangedAttacks.Count; i++)
        {
            rangedAttacks[i] += Time.fixedDeltaTime;
        }

        ranged_AttackTimer += Time.fixedDeltaTime;
        chaseTimer += Time.fixedDeltaTime;
        waitingTimer += Time.fixedDeltaTime;

        stunTime -= Time.fixedDeltaTime;

        if (stunTime > 0f)
        {
            return;
        } else
        {
            if (!agent.enabled)
            {
                agent.enabled = true;
            } 
        }


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

        bool hunting = false;

        if (waitingThresdhold >= DayNightCycle2D.DayDuration)
        {
            hunting = true;
        }

        if (detectTimer >= 0f && waitingTimer >= 0f)
        {
            Transform detected = FindPlayer(hunting);
            if (detected != null && (CheckIfNear(detected.position, data.Detection_Range) || hunting))
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

            if (target != null)
            {
                if (data.cloacked)
                {
                    photonView.RPC("RPC_ToggleHidden", RpcTarget.All, CheckIfNear(target.position, data.unCloakRange));
                }

                if (data.holdEnemy)
                {
                    if (CheckIfNear(target.position, data.holdRange))
                    {
                        Player player = target.GetComponent<Player>();
                        if (player)
                        {
                            player.Stunned(data.Detection_Rate + 0.5f);
                        }
                    }
                }
            }
        }

        if (target != null)
        {
            if (meleeAttacks.Count > 0)
            {
                for (int i = 0; i < meleeAttacks.Count; i++)
                {
                    EnemyMeleeAttack enemyMeleeAttack = data.meleeAttacks[i];
                    if (meleeAttacks[i] >= 0)
                    {
                        if (CheckIfNear(target.position, enemyMeleeAttack.Range))
                        {
                            waitingThresdhold = 0;
                            meleeAttacks[i] =-enemyMeleeAttack.Attack_Rate;
                            waitingTimer = -data.WaitTime;

                            GameManager.SummonAttackArea(
                                transform.position,
                                UnityEngine.Quaternion.Euler(0, 0, enemy.lookDir),
                                new AreaInstance(enemyMeleeAttack.Damage, enemyMeleeAttack.Attack_KB, enemyMeleeAttack.Attack_KBDuration, enemyMeleeAttack.Attack_Effect, enemyMeleeAttack.Attack_Hitbox, Game.g_players.transform)
                                );

                            agent.enabled = false;
                        }
                    }
                }
            }

            if (rangedAttacks.Count>0)
            {
                for (int i = 0; i < rangedAttacks.Count; i++)
                {
                    EnemyRangedAttack enemyRangedAttack = data.rangedAttacks[i];
                    if (rangedAttacks[i] >= 0)
                    {
                        if (CheckIfNear(target.position, enemyRangedAttack.Range))
                        {
                            waitingThresdhold = 0;
                            rangedAttacks[i] = -enemyRangedAttack.Attack_Rate;
                            waitingTimer = -data.WaitTime;

                            for (int j = 0; j < enemyRangedAttack.ranged_clipSize; j++)
                            {
                                float look = enemy.lookDir + UnityEngine.Random.Range(-enemyRangedAttack.ranged_spread, enemyRangedAttack.ranged_spread);

                                GameManager.SummonProjectile(transform.gameObject,
                                    transform.position,
                                    UnityEngine.Quaternion.Euler(0, 0, look),
                                    new ProjectileData(
                                        enemyRangedAttack.ranged_bulletSpeed,
                                        enemyRangedAttack.Damage,
                                        enemyRangedAttack.Attack_KB,
                                        enemyRangedAttack.Attack_KBDuration,
                                        enemyRangedAttack.ranged_bulletLifetime,
                                        enemyRangedAttack.Attack_Effect,
                                        enemyRangedAttack.ranged_projectileDat,
                                        Game.g_players.transform
                                    ),
                                    enemyRangedAttack.ranged_projectile
                                );

                                photonView.RPC("RPC_RangedAttack", RpcTarget.Others, i, look);
                            }

                            agent.enabled = false;
                        }
                    }
                }
            }
        }

        // Di chuyển đến target nếu không chờ
        if (waitingTimer >= 0f && target != null)
        {
            if (!agent.enabled) agent.enabled = true;
            agent.SetDestination(targetPos);
        }


        if (agent.enabled)
        {
            enemy.lookAtPos = targetPos;

            UnityEngine.Vector2 direction = (agent.steeringTarget - (UnityEngine.Vector3)rb.position);
            rb.linearVelocity = direction.normalized * data.Speed;
        }
    }
}