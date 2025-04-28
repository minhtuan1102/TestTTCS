using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button offlineButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private TMP_Text statusText;

    private bool isOfflineMode;
    private float connectionTimeout = 10f; // Timeout sau 10 giây

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        loginButton.interactable = false;
        playerNameInput.onValueChanged.AddListener(OnNameInputChanged);
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        offlineButton.onClick.AddListener(() => SetMode(true));
        multiplayerButton.onClick.AddListener(() => SetMode(false));

        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime))
        {
            statusText.text = "Lỗi: Chưa thiết lập Photon App ID. Vui lòng kiểm tra trong PUN Wizard.";
            return;
        }

        statusText.text = "Vui lòng nhập tên và chọn chế độ.";
    }

    private void SetMode(bool offline)
    {
        isOfflineMode = offline;
        statusText.text = offline ? "Chế độ ngoại tuyến được chọn" : "Chế độ trực tuyến được chọn";
        ValidateLoginInput();
    }

    private void OnNameInputChanged(string input) => ValidateLoginInput();

    private void ValidateLoginInput()
    {
        loginButton.interactable = !string.IsNullOrEmpty(playerNameInput.text) && playerNameInput.text.Length >= 3;
    }

    private void OnLoginButtonClicked()
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetInt("GameMode", isOfflineMode ? 1 : 0);

        if (isOfflineMode)
        {
            StartCoroutine(SwitchToOfflineModeAndLoadGame());
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                statusText.text = "Không có kết nối internet. Vui lòng kiểm tra mạng.";
                return;
            }

            statusText.text = "Đang kết nối đến Photon...";
            loginButton.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(ConnectionTimeoutCoroutine());
        }
    }

    private System.Collections.IEnumerator ConnectionTimeoutCoroutine()
    {
        yield return new WaitForSeconds(connectionTimeout);
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "Lỗi: Không thể kết nối tới Photon Cloud. Vui lòng kiểm tra mạng.";
            loginButton.interactable = true;
        }
    }

    private IEnumerator SwitchToOfflineModeAndLoadGame()
    {
        statusText.text = "Đang chuyển sang chế độ offline...";

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected) yield return null;
        }

        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene("Game");
    }

    public override void OnConnectedToMaster()
    {
        if (!isOfflineMode)
        {
            statusText.text = "Đã kết nối tới Photon, vào lobby...";
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        loginButton.interactable = true;

        string errorMessage = cause switch
        {
            DisconnectCause.Exception => "Lỗi mạng không xác định. Vui lòng kiểm tra kết nối internet.",
            DisconnectCause.ServerTimeout => "Hết thời gian kết nối tới server. Vui lòng thử lại sau.",
            DisconnectCause.ClientTimeout => "Hết thời gian từ phía client. Vui lòng kiểm tra kết nối mạng.",
            _ => $"Mất kết nối: {cause}. Vui lòng thử lại."
        };

        statusText.text = errorMessage;

        if (!isOfflineMode)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}