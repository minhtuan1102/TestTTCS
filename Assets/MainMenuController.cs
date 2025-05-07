//using Photon.Pun;
//using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(SwitchToOfflineMode());
    }

    private IEnumerator SwitchToOfflineMode()
    {
        // Ngắt kết nối nếu đang kết nối
        //if (PhotonNetwork.IsConnected)
        //{
        //    PhotonNetwork.Disconnect();

        // Chờ đến khi ngắt kết nối hoàn tất
        //    while (PhotonNetwork.IsConnected)
        //   {
        //     yield return null;
        //   }
        //}

        // Kích hoạt chế độ offline
        //PhotonNetwork.OfflineMode = true;

        // Tạo phòng offline (tự động)
        //PhotonNetwork.CreateRoom("OfflineRoom", new RoomOptions { MaxPlayers = 1 });
        return null;
    }
}
