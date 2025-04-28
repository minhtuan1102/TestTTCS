using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [Header("Cài đặt kẻ thù")]
    public GameObject[] enemyPrefabs;
    public int maxEnemies = 10;
    public float spawnRadius = 10f;
    public float spawnDelay = 2f;

    [Header("Cài đặt vũ khí")]
    public GameObject[] weaponPrefabs;
    public int maxWeapons = 3;
    public float weaponSpawnRadius = 5f;

    [Header("Cài đặt vật phẩm")]
    public GameObject[] itemPrefabs;
    public int maxItems = 5;
    public float itemSpawnRadius = 5f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeWeapons = new List<GameObject>();
    private List<GameObject> activeItems = new List<GameObject>();

    private bool isSpawningStarted = false;

    void Start()
    {
        TryStartSpawning();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        TryStartSpawning();
    }

    private void TryStartSpawning()
    {
        if (CanSpawn() && !isSpawningStarted)
        {
            StartSpawning();
        }
    }

    private bool CanSpawn()
    {
        return PhotonNetwork.OfflineMode || (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom);
    }

    private void StartSpawning()
    {
        isSpawningStarted = true;
        Debug.Log("Bắt đầu spawn!");

        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnDelay);

        for (int i = 0; i < maxWeapons; i++) SpawnWeapon();
        for (int i = 0; i < maxItems; i++) SpawnItem();
    }

    void SpawnEnemy()
    {
        if (!CanSpawn() || activeEnemies.Count >= maxEnemies) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // Spawn RoomObject để tồn tại dù MasterClient out
        GameObject enemy = PhotonNetwork.InstantiateRoomObject(GetRandom(enemyPrefabs).name, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    public void SpawnWeapon()
    {
        if (!CanSpawn() || activeWeapons.Count >= maxWeapons) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * weaponSpawnRadius;

        GameObject weapon = PhotonNetwork.InstantiateRoomObject(GetRandom(weaponPrefabs).name, spawnPos, Quaternion.identity);
        activeWeapons.Add(weapon);
    }

    public void SpawnItem()
    {
        if (!CanSpawn() || activeItems.Count >= maxItems) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * itemSpawnRadius;

        GameObject item = PhotonNetwork.InstantiateRoomObject(GetRandom(itemPrefabs).name, spawnPos, Quaternion.identity);
        activeItems.Add(item);
    }

    T GetRandom<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (activeEnemies.Remove(enemy))
        {
            // Chỉ Master Client destroy
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(enemy);

            Debug.Log("Enemy defeated.");
        }
    }

    public void WeaponPickedUp(GameObject weapon)
    {
        if (activeWeapons.Remove(weapon))
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(weapon);

            if (CanSpawn())
                SpawnWeapon();
        }
    }

    public void ItemPickedUp(GameObject item)
    {
        if (activeItems.Remove(item))
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(item);

            if (CanSpawn())
                SpawnItem();
        }
    }
}
