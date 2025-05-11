using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Items/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeReference] public string ID;
    [SerializeReference] public int Point = 100;

    [SerializeReference] public float Health = 80;
    [SerializeReference] public float Speed = 5f;

    [SerializeReference] public float Damage = 10f;
    [SerializeReference] public float Range = 5f;
    [SerializeReference] public float Attack_Rate = 1f;

    [SerializeReference] public GameObject Attack_Hitbox;

    [SerializeReference] public float WaitTime = 0.5f;

    [SerializeReference] public GameObject EnemyModel;

    [SerializeReference] public float Detection_Range = 10f;
    [SerializeReference] public float Detection_Rate = 0.2f;

    [SerializeReference] public float Chasing_Time_Threshold = 0.2f;

}
