using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Items/ProjectileItem")]
public class ProjectileItem : ScriptableObject
{
    public float speed;
    public float damage = 0f;
    public float lifeTime = 2f;

    public GameObject player;
}