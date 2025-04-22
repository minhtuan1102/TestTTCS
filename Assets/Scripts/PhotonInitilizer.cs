using UnityEngine;
using Photon.Pun;

public class PhotonInitializer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true; // Đảm bảo Photon chạy offline
            PhotonNetwork.CreateRoom("OfflineRoom"); // Tạo phòng offline để hoạt động RPC
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Đã vào phòng offline!");
        // Bạn có thể spawn player hoặc các vật phẩm ở đây
        // PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity);
    }
}
