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

    [Header("Item Settings")]
    public GameObject[] itemPrefabs; // Thêm mảng prefab cho vật phẩm khác
    public int maxItems = 5;
    public float itemSpawnRadius = 5f; // Khoảng cách spawn cho vật phẩm

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeWeapons = new List<GameObject>();
    private List<GameObject> activeItems = new List<GameObject>(); // Danh sách lưu trữ các vật phẩm đang tồn tại

    void Start()
    {
        // Bắt đầu spawn kẻ địch và vũ khí
        InvokeRepeating("SpawnEnemy", 0f, spawnDelay);
        SpawnInitialWeapons();
        SpawnInitialItems(); // Thêm spawn các vật phẩm ban đầu
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

    void SpawnInitialItems() // Hàm spawn các vật phẩm ban đầu
    {
        for (int i = 0; i < maxItems; i++)
        {
            SpawnItem();
        }
    }

    public void SpawnWeapon()
    {
        if (activeWeapons.Count >= maxWeapons) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * weaponSpawnRadius;
        GameObject weapon = Instantiate(GetRandomWeapon(), spawnPos, Quaternion.identity);
        activeWeapons.Add(weapon);
    }

    public void SpawnItem() // Hàm spawn vật phẩm
    {
        if (activeItems.Count >= maxItems) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * itemSpawnRadius;
        GameObject item = Instantiate(GetRandomItem(), spawnPos, Quaternion.identity);
        activeItems.Add(item);
    }

    GameObject GetRandomEnemy()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    GameObject GetRandomWeapon()
    {
        return weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
    }

    GameObject GetRandomItem() // Hàm chọn ngẫu nhiên vật phẩm
    {
        return itemPrefabs[Random.Range(0, itemPrefabs.Length)];
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

    // Gọi khi vật phẩm được nhặt
    public void ItemPickedUp(GameObject item)
    {
        if (activeItems.Contains(item))
        {
            activeItems.Remove(item);
            SpawnItem(); // Spawn vật phẩm mới ngay lập tức
        }
    }
}
