using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeReference] public float health = 5f;
    public EnemyData myDataRefer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (myDataRefer != null)
        {
            health = myDataRefer.Health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
