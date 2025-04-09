using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/ProjectileItem")]
public class ProjectileItem : ScriptableObject
{
    public float speed;         // Trọng lượng của vật phẩm
    public float damage = 0f;
    public float lifeTime = 2f;

}