using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [Header("UI Thông tin người chơi")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button offlineButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private TMP_Text statusText;

    [Header("UI Phòng")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_Text roomStatusText;

    [Header("Cài đặt")]
    public string gameSceneName = "Game";
    public string menuSceneName = "Menu";
    public string defaultRoomName = "DefaultRoom";
    public int maxPlayers = 4;

    private bool isOfflineMode = false;
    private bool isInLobby = false;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        loginButton.interactable = false;
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        playerNameInput.onValueChanged.AddListener(delegate { ValidateLoginInput(); });
        loginButton.onClick.AddListener(OnLoginClicked);
        offlineButton.onClick.AddListener(() => SetMode(true));
        multiplayerButton.onClick.AddListener(() => SetMode(false));
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);

        statusText.text = "Nhập tên và chọn chế độ chơi.";
    }

    private void SetMode(bool offline)
    {
        isOfflineMode = offline;
        statusText.text = offline ? "Đã chọn chế độ offline." : "Đã chọn chế độ online.";
        ValidateLoginInput();
    }

    private void ValidateLoginInput()
    {
        loginButton.interactable = !string.IsNullOrEmpty(playerNameInput.text) && playerNameInput.text.Length >= 3;
    }

    private void OnLoginClicked()
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetInt("GameMode", isOfflineMode ? 1 : 0);

        if (isOfflineMode)
        {
            StartCoroutine(SwitchToOfflineModeAndLoadGame());
        }
        else
        {
            statusText.text = "Đang kết nối đến Photon...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private IEnumerator SwitchToOfflineModeAndLoadGame()
    {
        statusText.text = "Chuyển sang chế độ offline...";
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected) yield return null;
        }

        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene(gameSceneName);
    }

    // ===== Online Callbacks =====

    public override void OnConnectedToMaster()
    {
        statusText.text = "Đã kết nối đến Photon!";
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        isInLobby = true;
        statusText.text = "Vào lobby thành công. Tạo hoặc tham gia phòng.";
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnCreateRoomClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !isInLobby)
        {
            roomStatusText.text = "Chưa vào lobby.";
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? defaultRoomName : roomNameInput.text;
        RoomOptions options = new RoomOptions { MaxPlayers = (byte)maxPlayers, IsOpen = true, IsVisible = true };

        roomStatusText.text = $"Đang tạo phòng: {roomName}";
        PhotonNetwork.CreateRoom(roomName, options);
    }

    private void OnJoinRoomClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !isInLobby)
        {
            roomStatusText.text = "Chưa vào lobby.";
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? defaultRoomName : roomNameInput.text;
        roomStatusText.text = $"Đang tham gia phòng: {roomName}";
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        roomStatusText.text = "Đã vào phòng. Đang chuyển scene...";
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
        statusText.text = $"Mất kết nối: {cause}";
        loginButton.interactable = true;
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        if (!isOfflineMode)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
