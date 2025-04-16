using UnityEngine;
using Photon.Pun;

public class ItemPickup : MonoBehaviourPun, IPunObservable
{
    [SerializeReference] private float time_between = 3f;
    private GameObject items_collection;
    private bool can_Picked_Up = true;
    private float timer = 0f;

    private Transform playerHolder;
    private float fireCooldown = 0f;

    [SerializeReference] public Item itemData;

    private void Start()
    {
        items_collection = GameObject.Find("Items");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;
        if (other != null && can_Picked_Up && other.CompareTag("Player"))
        {
            Transform player_Hand = other.transform.Find("Main");
            if (player_Hand.transform.childCount == 0)
            {
                playerHolder = other.transform;
                photonView.RPC("PickupItem", RpcTarget.All, playerHolder.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    [PunRPC]
    void PickupItem(int playerViewID)
    {
        PhotonView playerView = PhotonView.Find(playerViewID);
        if (playerView == null) return;

        playerHolder = playerView.transform;
        Transform player_Hand = playerHolder.Find("Main");
        transform.position = player_Hand.position;
        transform.SetParent(player_Hand);
        Vector3 localScale = transform.localScale;
        localScale.y = Mathf.Abs(localScale.y);
        transform.localScale = localScale;
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        Player playerScript = playerHolder.GetComponent<Player>();
        playerScript.swingOffset = itemData.swingOffset;

        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            string type = gameObject.name.Contains("Weapon") ? "Weapon" : "Item";
            if (type == "Weapon") spawnManager.WeaponPickedUp(gameObject);
            else spawnManager.ItemPickedUp(gameObject);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0) can_Picked_Up = true;

        if (!photonView.IsMine) return;

        if (playerHolder != null)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Drop();
            }
        }

        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            if (playerHolder != null && fireCooldown <= 0f)
            {
                Vector2 fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                Player playerScript = playerHolder.GetComponent<Player>();

                Melee Attack_Swing = GetComponent<Melee>();
                if (Attack_Swing != null)
                {
                    photonView.RPC("TriggerMelee", RpcTarget.All, itemData.damage, itemData.swing, itemData.recoil);
                }

                FireBullet Attack_Shoot = GetComponent<FireBullet>();
                if (Attack_Shoot != null && itemData.ranged)
                {
                    photonView.RPC("TriggerShoot", RpcTarget.All, itemData.damage, itemData.spread, itemData.fireAmount, itemData.recoil);
                }

                fireCooldown = itemData.cooldown;
            }
        }
    }

    [PunRPC]
    void TriggerMelee(float damage, float swing, float recoil)
    {
        Player playerScript = playerHolder.GetComponent<Player>();
        playerScript.TriggerRecoil(recoil);
        playerScript.TriggerSwing(swing);

        Melee Attack_Swing = GetComponent<Melee>();
        if (Attack_Swing != null)
        {
            Attack_Swing.TriggerAttack(damage);
        }
    }

    [PunRPC]
    void TriggerShoot(float damage, float spread, int fireAmount, float recoil)
    {
        Player playerScript = playerHolder.GetComponent<Player>();
        playerScript.TriggerRecoil(recoil);

        FireBullet Attack_Shoot = GetComponent<FireBullet>();
        if (Attack_Shoot != null)
        {
            Attack_Shoot.Shoot(damage, spread, fireAmount);
        }
    }

    public void Drop()
    {
        if (!photonView.IsMine) return;
        photonView.RPC("DropItem", RpcTarget.All);
    }

    [PunRPC]
    void DropItem()
    {
        Player playerScript = playerHolder.GetComponent<Player>();
        playerScript.swingOffset = 0f;

        can_Picked_Up = false;
        playerHolder = null;

        transform.SetParent(items_collection.transform);
        timer = -time_between;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(itemData.swingOffset);
            stream.SendNext(itemData.recoil);
            stream.SendNext(itemData.damage);
            stream.SendNext(itemData.swing);
            stream.SendNext(itemData.spread);
            stream.SendNext(itemData.fireAmount);
            stream.SendNext(itemData.cooldown);
        }
        else
        {
            itemData.swingOffset = (float)stream.ReceiveNext();
            itemData.recoil = (float)stream.ReceiveNext();
            itemData.damage = (float)stream.ReceiveNext();
            itemData.swing = (float)stream.ReceiveNext();
            itemData.spread = (float)stream.ReceiveNext();
            itemData.fireAmount = (int)stream.ReceiveNext();
            itemData.cooldown = (float)stream.ReceiveNext();
        }
    }
}