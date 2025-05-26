using NUnit.Framework.Interfaces;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AreaAttack : MonoBehaviour
{
    public AreaInstance data;
    GameObject hitbox;

    private Collider2D attackCollider;

    private LayerMask enemyLayer;

    public void Initiate()
    {
        hitbox = Instantiate(data.collider, transform);

        attackCollider = hitbox.GetComponent<PolygonCollider2D>();

        enemyLayer = (1 << data.group.gameObject.layer);
    }

    public void Knockback(Rigidbody2D rb, float kb)
    {
        if (rb == null) return;
        Vector2 direction = (rb.position - new Vector2(transform.position.x, transform.position.y)).normalized;
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * kb, ForceMode2D.Impulse);
    }

    public void Attack()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[10];
        int hitCount = attackCollider.Overlap(filter, results);

        for (int i = 0; i < hitCount; i++)
        {
            if (results[i].transform.IsChildOf(data.group))
            {
                HealthSystem health = results[i].GetComponent<HealthSystem>();

                health.TakeDamage((int)data.damage);

                EnemyAI enemyAI = results[i].GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Stunned(1f);
                }

                Knockback(results[i].GetComponent<Rigidbody2D>(), data.kb);

                foreach (DamageEffect effect in data.effects.ToList<DamageEffect>())
                {
                    health.addEffect(effect);
                }
            }
        }

        Destroy(gameObject);
    }
}
