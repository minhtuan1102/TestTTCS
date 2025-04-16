using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPunCallbacks
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
    public GameObject[] itemPrefabs;
    public int maxItems = 5;
    public float itemSpawnRadius = 5f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeWeapons = new List<GameObject>();
    private List<GameObject> activeItems = new List<GameObject>();

    void Awake()
    {
        Debug.Log("SpawnManager khởi tạo!");
    }

    void Start()
    {
        Debug.Log("Enemy Prefabs: " + (enemyPrefabs != null ? enemyPrefabs.Length : 0));
        Debug.Log("Weapon Prefabs: " + (weaponPrefabs != null ? weaponPrefabs.Length : 0));
        Debug.Log("Item Prefabs: " + (itemPrefabs != null ? itemPrefabs.Length : 0));

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Bắt đầu spawn...");
            InvokeRepeating("SpawnEnemy", 0f, spawnDelay);
            SpawnInitialWeapons();
            SpawnInitialItems();
        }
        else
        {
            Debug.Log("Không phải MasterClient, không spawn.");
        }
    }

    void SpawnEnemy()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (activeEnemies.Count >= maxEnemies) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        string enemyPath = "Enemy/" + GetRandomEnemy().name;
        Debug.Log("Đang spawn kẻ thù: " + enemyPath + " tại vị trí: " + spawnPos);
        try
        {
            GameObject enemy = PhotonNetwork.Instantiate(enemyPath, spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi spawn kẻ thù: " + e.Message);
        }
    }

    void SpawnInitialWeapons()
    {
        for (int i = 0; i < maxWeapons; i++)
        {
            SpawnWeapon();
        }
    }

    void SpawnInitialItems()
    {
        for (int i = 0; i < maxItems; i++)
        {
            SpawnItem();
        }
    }

    void SpawnWeapon()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (activeWeapons.Count >= maxWeapons) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * weaponSpawnRadius;
        string weaponPath = "Weapon/" + GetRandomWeapon().name;
        Debug.Log("Đang spawn vũ khí: " + weaponPath + " tại vị trí: " + spawnPos);
        try
        {
            GameObject weapon = PhotonNetwork.Instantiate(weaponPath, spawnPos, Quaternion.identity);
            activeWeapons.Add(weapon);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi spawn vũ khí: " + e.Message);
        }
    }

    void SpawnItem()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (activeItems.Count >= maxItems) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * itemSpawnRadius;
        string itemPath = "Items/" + GetRandomItem().name;
        Debug.Log("Đang spawn vật phẩm: " + itemPath + " tại vị trí: " + spawnPos);
        try
        {
            GameObject item = PhotonNetwork.Instantiate(itemPath, spawnPos, Quaternion.identity);
            activeItems.Add(item);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi spawn vật phẩm: " + e.Message);
        }
    }

    GameObject GetRandomEnemy()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    GameObject GetRandomWeapon()
    {
        return weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
    }

    GameObject GetRandomItem()
    {
        return itemPrefabs[Random.Range(0, itemPrefabs.Length)];
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public void WeaponPickedUp(GameObject weapon)
    {
        if (activeWeapons.Contains(weapon))
        {
            activeWeapons.Remove(weapon);
            photonView.RPC("SpawnNewWeapon", RpcTarget.All);
        }
    }

    public void ItemPickedUp(GameObject item)
    {
        if (activeItems.Contains(item))
        {
            activeItems.Remove(item);
            photonView.RPC("SpawnNewItem", RpcTarget.All);
        }
    }

    [PunRPC]
    void SpawnNewWeapon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnWeapon();
        }
    }

    [PunRPC]
    void SpawnNewItem()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnItem();
        }
    }
}