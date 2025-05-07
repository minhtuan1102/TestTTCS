using UnityEngine;
//using Photon.Pun;
//using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    /*
    [SerializeField] private GameObject playerPrefab; // Prefab của nhân vật
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 1, 0); // Vị trí spawn mặc định
    private bool hasSpawnedPlayer = false; // Biến kiểm tra xem đã spawn nhân vật chưa

    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.Log("Lỗi: Chưa thiết lập Prefab nhân vật trong NetworkManager!");
            return;
        }

        // Kiểm tra trạng thái kết nối Photon
        /*
        if (!PhotonNetwork.IsConnected)
        {
            // Nếu chưa kết nối, kết nối tới Photon
            if (PhotonNetwork.OfflineMode)
            {
                Debug.Log("Chế độ offline: Tạo phòng cục bộ...");
                JoinOrCreateRoom();
            }
            else
            {
                Debug.Log("Chế độ trực tuyến: Bắt đầu kết nối tới Photon Cloud...");
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            // Nếu đã kết nối, kiểm tra trạng thái
            if (PhotonNetwork.InRoom)
            {
                Debug.Log("Client đang ở trong phòng, rời phòng để quay lại lobby...");
                PhotonNetwork.LeaveRoom();
            }
            else if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby)
            {
                Debug.Log("Đã kết nối nhưng chưa vào lobby, tiến hành vào lobby...");
                PhotonNetwork.JoinLobby();
            }
            else if (PhotonNetwork.InLobby)
            {
                Debug.Log("Đã ở trong lobby, tiến hành tạo hoặc tham gia phòng...");
                JoinOrCreateRoom();
            }
            else
            {
                Debug.Log("Đang kết nối tới Photon, đợi callback OnConnectedToMaster hoặc OnJoinedLobby...");
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Đã kết nối tới Photon Cloud!");
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Đang vào lobby...");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Đã vào lobby thành công!");
        JoinOrCreateRoom();
    }

    void JoinOrCreateRoom()
    {
        // Kiểm tra xem client có đang ở trạng thái phù hợp không
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InLobby)
        {
            Debug.Log("Không thể tạo/tham gia phòng: Client chưa ở trong lobby. Đợi callback OnJoinedLobby...");
            return;
        }

        // Thiết lập tùy chọn phòng
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // Số người chơi tối đa
            IsVisible = true,
            IsOpen = true
        };

        // Tạo hoặc tham gia phòng
        string roomName = PhotonNetwork.OfflineMode ? "OfflineRoom" : "DefaultRoom";
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        Debug.Log($"Đang tạo hoặc tham gia phòng: {roomName}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Đã vào phòng: {PhotonNetwork.CurrentRoom.Name}!");

        // Kiểm tra scene và spawn nhân vật
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!hasSpawnedPlayer)
            {
                SpawnNhanVat();
                hasSpawnedPlayer = true;
            }
            else
            {
                Debug.Log("Nhân vật đã được spawn trước đó, không spawn lại.");
            }
        }
        else
        {
            Debug.Log($"Scene hiện tại ({SceneManager.GetActiveScene().name}) không phải là Game, không spawn nhân vật.");
            // Đợi scene tải xong rồi thử lại
            StartCoroutine(ChoTaiSceneGame());
        }
    }

    private System.Collections.IEnumerator ChoTaiSceneGame()
    {
        // Đợi cho đến khi scene Game được tải
        while (SceneManager.GetActiveScene().name != "Game")
        {
            Debug.Log("Đang chờ scene Game tải...");
            yield return new WaitForSeconds(0.5f);
        }

        // Khi scene Game đã tải, spawn nhân vật
        if (!hasSpawnedPlayer)
        {
            SpawnNhanVat();
            hasSpawnedPlayer = true;
        }
    }

    private void SpawnNhanVat()
    {
        // Kiểm tra xem Prefab có hợp lệ không

        if (playerPrefab == null)
        {
            Debug.Log("Lỗi: Prefab nhân vật chưa được thiết lập trong NetworkManager!");
            return;
        }

        // Kiểm tra xem Prefab có nằm trong thư mục Resources không
        string prefabName = playerPrefab.name;
        GameObject testPrefab = Resources.Load<GameObject>(prefabName);
        if (testPrefab == null)
        {
            Debug.Log($"Lỗi: Prefab {prefabName} không được tìm thấy trong thư mục Assets/Resources! Vui lòng kiểm tra lại.");
            return;
        }

        // Spawn nhân vật bằng PhotonNetwork.Instantiate
        try
        {
            GameObject nhanVat = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
            Debug.Log($"Đã spawn nhân vật {nhanVat.name} tại vị trí {spawnPosition}");
        }
        catch (System.Exception e)
        {
            Debug.Log($"Lỗi khi spawn nhân vật: {e.Message}");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Đã rời phòng, quay lại lobby...");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Tạo phòng thất bại với mã lỗi: {returnCode}, thông báo: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Tham gia phòng thất bại với mã lỗi: {returnCode}, thông báo: {message}");
    }

    /*public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Ngắt kết nối khỏi Photon với lý do: {cause}");
        hasSpawnedPlayer = false; // Reset để có thể spawn lại nếu kết nối lại
    }
    */
}