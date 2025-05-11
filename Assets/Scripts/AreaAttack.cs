using System.Diagnostics;
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
            }
        }

        Destroy(gameObject);
    }
}
