using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;

public class Generate : MonoBehaviour
{

    [SerializeReference] public int GeneratePoint = 1000;

    [SerializeReference] Button generateButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static List<EnemyData> enemies;
    public static GameObject enemy_Folder;

    private GameObject map;

    Vector2 GetRandomPositionInBarrier(Collider2D area, Vector2 enemyColliderSize)
    {
        Vector2 center = area.bounds.center;
        Vector2 size = area.bounds.size;

        float paddingX = enemyColliderSize.x / 2f;
        float paddingY = enemyColliderSize.y / 2f;

        float x = Random.Range(center.x - size.x / 2f + paddingX, center.x + size.x / 2f - paddingX);
        float y = Random.Range(center.y - size.y / 2f + paddingY, center.y + size.y / 2f - paddingY);

        return new Vector2(x, y);
    }

    EnemyData Randomized_EnemySpawn(int point)
    {
        List<EnemyData> Spawnable = new List<EnemyData>();

        foreach (EnemyData e in enemies)
        {
            if (point >= e.Point)
            {
                Spawnable.Add(e);
            }
        }

        if (Spawnable.Count == 0) return null;
        return Spawnable[Random.Range(0, Spawnable.Count)];
    } 

    public void GenerateRound()
    {
        int locationCount = map.transform.childCount;
        int locationPoint = (int) Mathf.Floor(GeneratePoint / locationCount);
        foreach (Transform location in map.transform)
        {
            int Point = locationPoint;
            while (Point > 0)
            {
                EnemyData spawnEnemy = Randomized_EnemySpawn(locationPoint);

                if (spawnEnemy == null) break;
                Point -= spawnEnemy.Point;

                Vector2 enemyScale = Vector2.Scale(spawnEnemy.EnemyModel.transform.GetComponent<BoxCollider2D>().size, spawnEnemy.EnemyModel.transform.localScale);

                GameObject enemy = Instantiate(spawnEnemy.EnemyModel, GetRandomPositionInBarrier(location.GetComponent<BoxCollider2D>(), enemyScale), Quaternion.Euler(0, 0, 0), enemy_Folder.transform);
                enemy.GetComponent<Enemy>().myDataRefer = spawnEnemy;           
            }
            
        }
    }

    void ButtonGenerate()
    {
        GenerateRound();
    }

    void Start()
    {
        map = GameObject.Find("Map");
        enemies = new List<EnemyData>(Resources.LoadAll<EnemyData>("Items/Enemy"));
        enemy_Folder = GameObject.Find("Enemies");
        generateButton.onClick.AddListener(ButtonGenerate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
