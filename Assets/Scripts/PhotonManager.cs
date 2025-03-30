using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.Instantiate("PlayerPrefab", new Vector2(1, 1), Quaternion.identity);
    }
}