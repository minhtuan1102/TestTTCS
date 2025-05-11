
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System;

public class PlayerEntryUI : MonoBehaviourPunCallbacks
{
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image playerAvatar;
    public Sprite[] avatars;
    public GameObject leftArrow;
    public GameObject rightArrow;

    Photon.Realtime.Player player;
    public void SetPlayerInfo(Photon.Realtime.Player _player)
    {
        player = _player;
        UpdatePlayerItem(player);
    }
    private void ChangeAvatar(int direction)
    {
        if (avatars.Length == 0)
        {
            Debug.LogError("Mảng avatars không có phần tử.");
            return;
        }

        int currentAvatar = playerProperties.ContainsKey("playerAvatar") ? (int)playerProperties["playerAvatar"] : 0;

        currentAvatar = (currentAvatar + direction + avatars.Length) % avatars.Length;

        playerProperties["playerAvatar"] = currentAvatar;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);

        Debug.Log($"Avatar đã thay đổi sang {currentAvatar}.");
        playerAvatar.sprite = avatars[currentAvatar];
        Debug.Log($"Cập nhật avatar: {playerAvatar.sprite.name}");
    }
    public void OnclickLeftArrow()
    {
        Debug.Log("Nhấn nút mũi tên trái");
        ChangeAvatar(-1);
    }
    public void OnClickRightArrow()
    {
        Debug.Log("Nhấn nút mũi tên phải");
        ChangeAvatar(1);
    }

    // Cập nhật thông tin người chơi khi có sự thay đổi thuộc tính
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    // Cập nhật avatar người chơi
    void UpdatePlayerItem(Photon.Realtime.Player targetPlayer)
    {
        if (targetPlayer.CustomProperties.ContainsKey("playerAvatar"))
        {
            int avatarIndex = (int)targetPlayer.CustomProperties["playerAvatar"];
            playerAvatar.sprite = avatars[avatarIndex];
            Debug.Log($"Cập nhật avatar từ Photon: {playerAvatar.sprite.name}");
            playerProperties["playerAvatar"] = avatarIndex;
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
    }
}