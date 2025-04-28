using UnityEngine;
using Photon.Pun;

public class StaminaPickup : MonoBehaviourPun
{
    [SerializeField] private float staminaAmount = 30f;
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            Player player = other.GetComponent<Player>();
            PlayerStamina stamina = other.GetComponent<PlayerStamina>();

            if (player != null && stamina != null && stamina.currentStamina < stamina.maxStamina && player.photonView.IsMine)
            {
                photonView.RPC("RPC_Pickup", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_Pickup()
    {
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        spawnManager.ItemPickedUp(gameObject);
        PhotonNetwork.Destroy(gameObject);
    }
}