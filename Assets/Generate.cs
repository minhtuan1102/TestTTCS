using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class Generate : MonoBehaviour
{

    [SerializeReference] public int GeneratePoint = 1000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static List<EnemyData> enemies;
    private GameObject map;

    public void GenerateRound()
    {
        int locationCount = map.transform.childCount;
        int locationPoint = (int) Mathf.Floor(GeneratePoint / locationCount);
        for (int i = 1; i <= map.transform.childCount; i++)
        {



        }
    }

    void Start()
    {
        map = GameObject.Find("Map");
        enemies = new List<EnemyData>(Resources.LoadAll<EnemyData>("Items"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
