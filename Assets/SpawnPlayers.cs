using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private void Start()
    {
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        newPlayer.transform.SetParent(GameObject.Find("Players").transform);

        newPlayer.SetActive(true);
    }
}
