using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [Header("Cài đặt phòng")]
    public string gameSceneName = "Game";
    public int maxPlayers = 4;
    public string defaultRoomName = "DefaultRoom";

    [Header("UI Tham chiếu")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_Text roomStatusText;

    [SerializeField] private GameObject roomListPanel;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListEntryPrefab;

    private bool isInLobby = false;
    private bool isShowingRoomList = false;
    private float connectionTimeout = 10f;

    private Dictionary<string, GameObject> roomEntries = new Dictionary<string, GameObject>();

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        roomStatusText.text = "Đang kết nối tới Photon Cloud...";

        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        roomListPanel.SetActive(false);

        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime))
        {
            roomStatusText.text = "Lỗi: Photon App ID chưa thiết lập.";
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            roomStatusText.text = "Không có kết nối internet.";
            return;
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(ConnectionTimeoutCoroutine());
        }
        else if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
    }

    private System.Collections.IEnumerator ConnectionTimeoutCoroutine()
    {
        yield return new WaitForSeconds(connectionTimeout);
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            roomStatusText.text = "Không thể kết nối tới Photon Cloud.";
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
        }
    }

    private void OnCreateRoomClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !isInLobby)
        {
            roomStatusText.text = "Chưa vào lobby.";
            return;
        }

        // Lấy tên phòng từ input hoặc tạo tên duy nhất
        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? GenerateUniqueRoomName() : roomNameInput.text;

        // Kiểm tra xem tên phòng đã tồn tại chưa
        if (roomEntries.ContainsKey(roomName))
        {
            roomStatusText.text = $"Phòng {roomName} đã tồn tại. Vui lòng chọn tên khác.";
            return;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true
        };

        roomStatusText.text = $"Đang tạo phòng {roomName}...";
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // Hàm tạo tên phòng duy nhất
    private string GenerateUniqueRoomName()
    {
        string baseName = defaultRoomName;
        string uniqueId = Random.Range(1000, 9999).ToString(); // Thêm số ngẫu nhiên
        return $"{baseName}_{uniqueId}";
    }
    private void OnJoinRoomClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !isInLobby)
        {
            roomStatusText.text = "Chưa vào lobby.";
            return;
        }

        if (!isShowingRoomList)
        {
            // Lần đầu bấm tham gia -> Hiện danh sách phòng
            roomStatusText.text = "Chọn 1 phòng để tham gia:";
            roomListPanel.SetActive(true);
            isShowingRoomList = true;
        }
        else
        {
            if (string.IsNullOrEmpty(roomNameInput.text))
            {
                roomStatusText.text = "Hãy nhập tên phòng.";
                return;
            }

            roomStatusText.text = $"Đang tham gia phòng {roomNameInput.text}...";
            PhotonNetwork.JoinRoom(roomNameInput.text);
        }
    }

    public override void OnConnectedToMaster()
    {
        roomStatusText.text = "Đã kết nối tới Photon Cloud!";
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        isInLobby = true;
        roomStatusText.text = "Đã vào lobby.";
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Đảm bảo các biến cần thiết không bị null
        if (roomEntries == null)
            roomEntries = new Dictionary<string, GameObject>();

        if (roomListEntryPrefab == null || roomListContent == null)
        {
            Debug.LogError("RoomListEntryPrefab hoặc RoomListContent chưa được gán trong Inspector!");
            return;
        }

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                // Nếu phòng bị xóa (ví dụ đóng phòng) thì gỡ ra khỏi danh sách
                if (roomEntries.ContainsKey(roomInfo.Name))
                {
                    Destroy(roomEntries[roomInfo.Name]);
                    roomEntries.Remove(roomInfo.Name);
                }
            }
            else
            {
                if (!roomEntries.ContainsKey(roomInfo.Name))
                {
                    // Tạo mới một entry cho phòng
                    GameObject entry = Instantiate(roomListEntryPrefab, roomListContent);

                    // Đặt đúng tên phòng và số người chơi
                    TMP_Text roomNameText = entry.transform.Find("RoomNameText")?.GetComponent<TMP_Text>();
                    TMP_Text playerCountText = entry.transform.Find("PlayerCountText")?.GetComponent<TMP_Text>();
                    // Sau khi Instantiate entry prefab:
                    Button joinButton = entry.transform.Find("Button").GetComponent<Button>();
                    string capturedRoomName = roomInfo.Name; // Bắt roomName riêng cho từng button
                    joinButton.onClick.AddListener(() => JoinSelectedRoom(capturedRoomName));


                    if (roomNameText != null)
                        roomNameText.text = roomInfo.Name;

                    if (playerCountText != null)
                        playerCountText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";

                    if (joinButton != null)
                    {
                        string selectedRoomName = roomInfo.Name; // Phải lưu riêng ra biến, tránh lỗi lambda capture
                        joinButton.onClick.AddListener(() => JoinSelectedRoom(selectedRoomName));
                    }

                    roomEntries.Add(roomInfo.Name, entry);
                }
                else
                {
                    // Cập nhật số lượng người chơi nếu phòng đã có entry
                    TMP_Text playerCountText = roomEntries[roomInfo.Name].transform.Find("PlayerCountText")?.GetComponent<TMP_Text>();
                    if (playerCountText != null)
                        playerCountText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
                }
            }
        }
    }


    private void JoinSelectedRoom(string roomName)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            roomStatusText.text = "Không kết nối được tới Photon!";
            return;
        }

        roomStatusText.text = $"Đang tham gia phòng: {roomName}...";
        PhotonNetwork.JoinRoom(roomName);
    }


    public override void OnJoinedRoom()
    {
        roomStatusText.text = $"Đã vào phòng {PhotonNetwork.CurrentRoom.Name}. Đang vào game...";
        PhotonNetwork.LoadLevel(gameSceneName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomStatusText.text = $"Tạo phòng thất bại: {message}";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        roomStatusText.text = $"Tham gia phòng thất bại: {message}";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isInLobby = false;
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        string errorMessage = cause switch
        {
            DisconnectCause.Exception => "Lỗi mạng. Kiểm tra internet.",
            DisconnectCause.ServerTimeout => "Server timeout.",
            DisconnectCause.ClientTimeout => "Client timeout.",
            _ => $"Mất kết nối: {cause}"
        };

        roomStatusText.text = errorMessage;
        PhotonNetwork.ConnectUsingSettings();
    }
    public void OnClickCloseRoomList()
    {
        roomListPanel.SetActive(false);

        // Xoá hết phòng đang list để khi mở lại cập nhật mới
        foreach (var entry in roomEntries.Values)
        {
            Destroy(entry);
        }
        roomEntries.Clear();
    }
}
