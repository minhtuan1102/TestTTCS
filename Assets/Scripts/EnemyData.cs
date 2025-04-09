using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int Point = 100;

    public float Health = 80;
    public float Speed = 5f;

    public float Damage = 10f;
}
