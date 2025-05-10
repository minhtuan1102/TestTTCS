using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomEntryUI : MonoBehaviour
{
    public LobbyNetworkManager LobbyNetworkParent;
    [SerializeField] private TMP_Text _roomName;

    public void setName(string roomName)
    {
        _roomName.text = roomName;
    }

    public void OnJoinPressed()
    {
        LobbyNetworkParent.JoinRoom(_roomName.text);
    }
}
