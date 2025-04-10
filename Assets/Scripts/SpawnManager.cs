using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    public int maxEnemies = 10;
    public float spawnRadius = 10f;
    public float spawnDelay = 2f;

    [Header("Weapon Settings")]
    public GameObject[] weaponPrefabs;
    public int maxWeapons = 3;
    public float weaponSpawnRadius = 5f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeWeapons = new List<GameObject>();

    void Start()
    {
        // Bắt đầu spawn kẻ địch và vũ khí
        InvokeRepeating("SpawnEnemy", 0f, spawnDelay);
        SpawnInitialWeapons();
    }

    void SpawnEnemy()
    {
        if (activeEnemies.Count >= maxEnemies) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        GameObject enemy = Instantiate(GetRandomEnemy(), spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    void SpawnInitialWeapons()
    {
        for (int i = 0; i < maxWeapons; i++)
        {
            SpawnWeapon();
        }
    }

    public void SpawnWeapon()
    {
        if (activeWeapons.Count >= maxWeapons) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * weaponSpawnRadius;
        GameObject weapon = Instantiate(GetRandomWeapon(), spawnPos, Quaternion.identity);
        activeWeapons.Add(weapon);
    }

    GameObject GetRandomEnemy()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    GameObject GetRandomWeapon()
    {
        return weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
    }

    // Gọi khi enemy bị tiêu diệt
    public void EnemyDefeated(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    // Gọi khi vũ khí được nhặt
    public void WeaponPickedUp(GameObject weapon)
    {
        if (activeWeapons.Contains(weapon))
        {
            activeWeapons.Remove(weapon);
            SpawnWeapon(); // Spawn vũ khí mới ngay lập tức
        }
    }
}