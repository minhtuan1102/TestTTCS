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

        object classIndex = null;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerAvatar", out classIndex);
        int _class = 0;
        if (classIndex != null) _class = (int)classIndex;

        object[] data = new object[] { _class };
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity, 0, data);
    }
}
