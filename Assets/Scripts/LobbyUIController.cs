using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class LobbyUIController : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public TMP_InputField roomNameInput;
    public Button createRoomButton;
    public Button refreshRoomListButton;
    public TMP_Text statusText;
    public GameObject roomListContent;
    public GameObject roomEntryPrefab;

    private string gameVersion = "0.9";
    private bool joiningRoom = false;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
        Connect();
    }

    void Connect()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected to Master Server";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby";
    }

    public void OnClickCreateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName)) return;

        joiningRoom = true;

        RoomOptions options = new RoomOptions { IsOpen = true, IsVisible = true, MaxPlayers = 10 };
        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public void OnClickRefreshRooms()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinLobby();
        else
            Connect();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent.transform)
            Destroy(child.gameObject);

        foreach (RoomInfo room in roomList)
        {
            GameObject entry = Instantiate(roomEntryPrefab, roomListContent.transform);
            entry.transform.Find("Text_RoomName").GetComponent<TMP_Text>().text = room.Name;
            entry.transform.Find("Text_PlayerCount").GetComponent<TMP_Text>().text = $"{room.PlayerCount}/{room.MaxPlayers}";

            Button joinBtn = entry.transform.Find("Button_Join").GetComponent<Button>();
            joinBtn.onClick.AddListener(() =>
            {
                joiningRoom = true;
                PhotonNetwork.NickName = playerNameInput.text;
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Failed to create room: " + message;
        joiningRoom = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Failed to join room: " + message;
        joiningRoom = false;
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = "Disconnected: " + cause.ToString();
    }
}
