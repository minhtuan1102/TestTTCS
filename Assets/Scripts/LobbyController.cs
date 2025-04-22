using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [Header("Cài đặt phòng")]
    public string gameSceneName = "Game";
    public int maxPlayers = 4;
    public string defaultRoomName = "DefaultRoom";

    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_Text roomStatusText;

    private bool isInLobby = false;
    private float connectionTimeout = 10f; // Timeout sau 10 giây

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        roomStatusText.text = "Đang kết nối tới Photon Cloud...";

        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime))
        {
            roomStatusText.text = "Lỗi: Photon App ID không được thiết lập. Vui lòng kiểm tra trong PUN Wizard.";
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            roomStatusText.text = "Không có kết nối internet. Vui lòng kiểm tra mạng.";
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
            roomStatusText.text = "Lỗi: Không thể kết nối tới Photon Cloud. Vui lòng kiểm tra kết nối mạng.";
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
        }
    }

    private void OnCreateRoomClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !isInLobby)
        {
            roomStatusText.text = "Chưa vào lobby. Đang thử lại...";
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? defaultRoomName : roomNameInput.text;
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true
        };

        roomStatusText.text = $"Đang tạo phòng {roomName}...";
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void OnJoinRoomClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !isInLobby)
        {
            roomStatusText.text = "Chưa vào lobby. Đang thử lại...";
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? defaultRoomName : roomNameInput.text;
        roomStatusText.text = $"Đang tham gia phòng {roomName}...";
        PhotonNetwork.JoinRoom(roomName);
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
        roomStatusText.text = "Đã vào lobby. Nhập tên phòng để tạo hoặc tham gia.";
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        roomStatusText.text = $"Đã vào phòng: {PhotonNetwork.CurrentRoom.Name}. Đang vào game...";
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
            DisconnectCause.Exception => "Lỗi mạng không xác định. Vui lòng kiểm tra kết nối internet.",
            DisconnectCause.ServerTimeout => "Hết thời gian kết nối tới server. Vui lòng thử lại sau.",
            DisconnectCause.ClientTimeout => "Hết thời gian từ phía client. Vui lòng kiểm tra kết nối mạng.",
            _ => $"Mất kết nối: {cause}. Đang thử lại..."
        };

        roomStatusText.text = errorMessage;
        PhotonNetwork.ConnectUsingSettings();
    }
}