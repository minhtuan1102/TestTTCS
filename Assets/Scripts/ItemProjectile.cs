using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Items/ProjectileItem")]
public class ProjectileItem : ScriptableObject
{
    public float damage = 0f;
    public float knockBack = 0f;

    public bool areaAttack = false;
    public GameObject hitbox;
    public List<DamageEffect> effects = new List<DamageEffect>();
}