using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerEntryUI : MonoBehaviour
{
    public LobbyNetworkManager LobbyNetworkParent;
    [SerializeField] private TMP_Text _playerName;

    public void setName(string roomName)
    {
        _playerName.text = roomName;
    }
}
