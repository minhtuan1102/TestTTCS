using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq.Expressions;
using Spine;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] public List<ItemInstance> Items;

    [SerializeField] public float pickUpRange = 2f;

    [SerializeField] public ItemInstance holdingItem;

    [SerializeField] public List<ItemInstance> Armor;
    [SerializeField] public List<string> currentWearing;

    private Transform nearestItem;

    private float scanTime = 0.2f;
    private float scanTimer = 0f;

    private string currentWeaponModel = "";
    private Transform weaponMuzzle;

    private HealthSystem healthSystem;
    private Player player;
    private PhotonView view;

    // Attack

    private int _itemIndex = 0;

    private float timer = 0f;
    private float atkCooldown = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        player = GetComponent<Player>();
        view = GetComponent<PhotonView>();
        healthSystem = GetComponent<HealthSystem>();

        for (int i=0; i<3; i++)
        {
            Armor.Add(null);
            currentWearing.Add("");
        }

        for  (int i=0; i<Items.Count; i++)
        {
            Items[i].itemID = i;
            _itemIndex = i + 1;
        }
    }

    void Start()
    {
        
    }

    [PunRPC]
    public void RPC_Holding(int itemIndex)
    {
        if (itemIndex == -1)
        {
            holdingItem = null;
        } else
        {
            ItemInstance item = GetItemInstanceFromID(itemIndex);
            if (item != null)
            {
                holdingItem = item;
            }
        }
    }

    public void Holding(ItemInstance item)
    {
        holdingItem = item;
        if (holdingItem != null)
        {
            if (Items.Contains(item))
            {
                view.RPC("RPC_Holding", RpcTarget.Others, item.itemID);
            }
        } else
        {
            view.RPC("RPC_Holding", RpcTarget.Others, -1);
        }
    }

    [PunRPC]
    public void RPC_Wearing(int itemIndex, int slot)
    {
        if (itemIndex == -1)
        {
            if (Armor[slot] != null)
            {
                Armor[slot] = null;
            }
        }
        else
        {
            ItemInstance item = GetItemInstanceFromID(itemIndex);
            if (item != null)
            {
                Armor[slot] = item;
            }
        }
        healthSystem.calculateArmor(Armor);
    }

    public void Wearing(ItemInstance item, int slot)
    {
        Armor[slot] = item;
        if (Armor[slot] != null)
        {
            if (Items.Contains(item))
            {
                view.RPC("RPC_Wearing", RpcTarget.Others, item.itemID, slot);
            }
        }
        else
        {
            view.RPC("RPC_Wearing", RpcTarget.Others, -1, slot);
        }
        healthSystem.calculateArmor(Armor);
    }

    // Game function

    public int FindItem(ItemInstance item)
    {
        int index = 0;
        for (index = 0; index < Items.Count; index++)
        {
            if (Items[index] != null)
            {
                if (item.itemRef.itemName == Items[index].itemRef.itemName)
                {
                    return index;
                }
            }
        }
        return -1;
    }

    public ItemInstance GetItemInstanceFromID(int id)
    {
        int index = 0;
        for (index = 0; index < Items.Count; index++)
        {
            if (Items[index] != null)
            {
                if (Items[index].itemID == id)
                {
                    return Items[index];
                }
            }
        }
        return null;
    }

    public int GetItemIndexFromID(int id)
    {
        int index = 0;
        for (index = 0; index < Items.Count; index++)
        {
            if (Items[index] != null)
            {
                if (Items[index].itemID == id)
                {
                    return index;
                }
            }
        }
        return -1;
    }

    // Update Item

    [PunRPC]
    private void RPC_UpdateItem(int id, string json)
    {
        ItemInstance data = new ItemInstance(JsonUtility.FromJson<ItemInstanceSender>(json));
       // Debug.Log(data.itemID);
       // Debug.Log(data.amount);
        int index = GetItemIndexFromID((int)id);

        //Debug.Log(index);

        Items[index].amount = data.amount;

        if (Items[index].amount <= 0)
        {
            Items[index].storage.gameObject.SetActive(false);
            Items.RemoveAt(index);
        }
    }

    // Drop Item

    [PunRPC]
    private void Master_DropItem(int id, int amount)
    {
        int index = GetItemIndexFromID (id);
        if (Items[index] != null)
        {
            if (Items[index].itemRef.isConsumable)
            {
                int DropAmount = (int)Mathf.Min(amount, Items[index].amount);
                GameManager.Instance.SpawnItem(Items[index].itemRef, DropAmount, transform.position, new Quaternion(0, 0, 0, 0));

                Items[index].amount -= DropAmount;
                view.RPC("RPC_UpdateItem", RpcTarget.Others, id, (new ItemInstanceSender(Items[index])).ToJson());
                if (Items[index].amount <= 0)
                {
                    Items.RemoveAt(index);
                }
            }
            else
            {
                GameManager.SpawnItem(Items[index], transform.position, new Quaternion(0, 0, 0, 0));
                Items[index].amount = 0;
                view.RPC("RPC_UpdateItem", RpcTarget.Others, id, (new ItemInstanceSender(Items[index])).ToJson());
                Items.RemoveAt(index);
            }
        }
    }

    public void DropItem(ItemInstance item, int amount)
    {
        if (Items.Contains(item)) { }
        else return;
            int index = Items.IndexOf(item);
        if (Items[index] != null)
        {
            if (Items[index].itemRef.isConsumable)
            {
                int DropAmount = (int)Mathf.Min(amount, Items[index].amount);
                Items[index].amount -= DropAmount;
                view.RPC("Master_DropItem", RpcTarget.MasterClient, Items[index].itemID, DropAmount);
            }
            else
            {
                view.RPC("Master_DropItem", RpcTarget.MasterClient, Items[index].itemID, Items[index].amount);
            }
        }
    }

    // Use Item

    [PunRPC]
    private void Master_UseItem(int id)
    {
        int index = GetItemIndexFromID(id);
        if (Items[index] != null)
        {
            if (Items[index].itemRef.isConsumable && Items[index].amount > 0)
            {
                foreach (Modify mod in Items[index].itemRef.consumeEffect)
                {
                    switch (mod.modify_ID)
                    {
                        case "HP":
                            HealthSystem health = player.GetComponent<HealthSystem>();
                            health.Heal(mod.modify_IntValue);
                            break;
                        case "MP":
                            player.GainMana((float)mod.modify_IntValue);
                            break;
                        default:
                            break;
                    }
                }

                Items[index].amount -= 1;
                view.RPC("RPC_UpdateItem", RpcTarget.Others, id, (new ItemInstanceSender(Items[index])).ToJson());
                if (Items[index].amount <= 0)
                {
                    Items.RemoveAt(index);
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    PlayerUI.UI.transform.GetComponent<PlayerUI>().UpdateInventory();
                }
            }
        }
    }

    public void UseItem(ItemInstance item)
    {
        if (Items.Contains(item)) { }
        else return;
        int index = Items.IndexOf(item);
        if (Items[index] != null)
        {
            if (Items[index].itemRef.isConsumable && Items[index].amount>0)
            {
                view.RPC("Master_UseItem", RpcTarget.MasterClient, item.itemID);

                if (!PhotonNetwork.IsMasterClient)
                {
                    Items[index].amount -= 1;
                    PlayerUI.UI.transform.GetComponent<PlayerUI>().UpdateInventory();
                }
            }
        }
    }

    // Add Item

    [PunRPC]
    private void Master_AddItem(string id)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Transform item in Game.g_items.transform)
            {
                if ((item.position - transform.position).magnitude < pickUpRange)
                {
                    ItemObject itemObject = item.GetComponent<ItemObject>();
                    ItemInstance itemData = itemObject.Data;
                    if (item.name == id)
                    {
                        AddItem(itemData);
                        PhotonNetwork.Destroy(item.gameObject);
                        view.RPC("Client_AddItem", RpcTarget.Others, new ItemInstanceSender(itemData).ToJson());
                        return;
                    }
                }
            }
        }
    }

    [PunRPC]
    private void Client_AddItem(string json)
    {
        AddItem(new ItemInstance(JsonUtility.FromJson<ItemInstanceSender>(json)));
    }

    public void AddItem(ItemInstance item)
    {
        int index = FindItem(item);
        if (index >= 0 && item.itemRef.isConsumable)
        {
            Items[index].amount += item.amount;
        }
        else
        {
            Items.Add(new ItemInstance(_itemIndex++, item));
        }
        if (view.IsMine)
        {
            if (PlayerUI.UI != null)
            {
                PlayerUI.UI.GetComponent<PlayerUI>().UpdateInventory();
            }
        }
    }

    //---------------------------------------------------------

    private Transform FindNearItem()
    {
        Transform nearest = null;
        float nearestDis = pickUpRange;
        float dis;
        if (Game.items != null)
        {
            foreach (Transform item in Game.g_items.transform)
            {
                dis = (item.position - transform.position).sqrMagnitude;
                if (dis < pickUpRange)
                {
                    if (nearest == null || (dis < nearestDis)) 
                    {
                        nearest = item;
                        nearestDis = dis;
                    }
                }
            }
        }

        return nearest;
    }

    void GetWeaponModel(string model)
    {
        if (model != currentWeaponModel)
        {
            currentWeaponModel = model;

            if (holdingItem != null && holdingItem.itemRef != null)
            {

                foreach (Transform item in player.HandItem.transform)
                {
                    Destroy(item.gameObject);
                }

                GameObject wp_model = Instantiate(holdingItem.itemRef.model, player.HandItem.transform);
                wp_model.name = "Model";

                weaponMuzzle = wp_model.transform.Find("Muzzle");
            }
        }
    }

    private void GetWearModel(ItemInstance wearing, Transform model, Sprite oldSprite)
    {
        foreach (Transform item in model)
        {
            Destroy(item.gameObject);
        }

        if (wearing != null)
        {
            if (wearing.itemRef.armor_modelType == ArmorModelType.Model)
            {
                GameObject wp_model = Instantiate(wearing.itemRef.armor_Model, model);
                wp_model.transform.localPosition = new Vector3(0, 0, 0);

                Vector3 scale = wp_model.transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                scale.y = Mathf.Abs(scale.y);
                wp_model.transform.localScale = scale;
            }
            else
            {
                model.GetComponent<SpriteRenderer>().sprite = wearing.itemRef.armor_Sprite;
            }
        } else
        {
            model.GetComponent<SpriteRenderer>().sprite = oldSprite;
        }
    }

    void GetArmorModel(int slot, string model)
    {
        if (currentWearing[slot] != model)
        {
            currentWearing[slot] = model;

            if (Armor[slot] != null && Armor[slot].itemRef != null)
            {

                switch (slot)
                {
                    case 0:
                        player.model_Hair.gameObject.SetActive(!Armor[slot].itemRef.hide_Hair);
                        GetWearModel(Armor[slot], player.model_Hat, null);
                        break;
                    case 1:
                        GetWearModel(Armor[slot], player.model_Body, player._class.Body);
                        break;
                    case 2:
                        GetWearModel(Armor[slot], player.model_Pant_L, player._class.Leg);
                        GetWearModel(Armor[slot], player.model_Pant_R, player._class.Leg);
                        break;
                    default:
                        break;
                }
            } else
            {
                switch (slot)
                {
                    case 0:
                        player.model_Hair.gameObject.SetActive(true);
                        GetWearModel(null, player.model_Hat, null);
                        break;
                    case 1:
                        GetWearModel(null, player.model_Body, player._class.Body);
                        break;
                    case 2:
                        GetWearModel(null, player.model_Pant_L, player._class.Leg);
                        GetWearModel(null, player.model_Pant_R, player._class.Leg);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void Update()
    {
        if (view.IsMine)
        {
            scanTimer += Time.fixedDeltaTime;

            if (scanTimer > 0f)
            {
                scanTimer -= scanTime;

                if (nearestItem != null)
                {
                    GameObject canvas = nearestItem.transform.Find("Canvas").gameObject;
                    canvas.SetActive(false);
                }

                nearestItem = FindNearItem();
            }

            if (nearestItem != null)
            {
                GameObject canvas = nearestItem.transform.Find("Canvas").gameObject;
                canvas.SetActive(true);

                canvas.transform.localPosition = new Vector3(0, 0.6f + 0.1f * Mathf.Cos(Time.time * 2), 0);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    view.RPC("Master_AddItem", RpcTarget.MasterClient, nearestItem.name);
                    nearestItem = null;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerUI.useMainWP = !(PlayerUI.useMainWP);
                PlayerUI.UI.transform.GetComponent<PlayerUI>().UpdateWP();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                PlayerUI.UI.transform.GetComponent<PlayerUI>().ToggleInventory();
            }
        }

        timer += Time.deltaTime;
        atkCooldown -= Time.deltaTime;

        if (holdingItem != null && holdingItem.itemRef)
        {
            player.swingOffset = holdingItem.itemRef.swingOffset;
            GetWeaponModel(holdingItem.itemRef.itemID);
            player.HandItem.gameObject.SetActive(true);
        }
        else
        {
            player.swingOffset = 0f;
            GetWeaponModel("");
            player.HandItem.gameObject.SetActive(false);
        }

        for (int i=0; i<3; i++)
        {
            if (Armor[i] != null && Armor[i].itemRef)
            {
                GetArmorModel(i, Armor[i].itemRef.itemID);
            }
            else
            {
                GetArmorModel(i, "");
            }
        }
    }

    [PunRPC]
    private void RPC_Attack(Vector3 pos, float look)
    {
        if (holdingItem != null && holdingItem.itemRef)
        {
            if (atkCooldown <= 0f && (holdingItem.ammo > 0 && holdingItem.itemRef.canShoot || holdingItem.itemRef.canMelee))
            {
                if (holdingItem.itemRef.canShoot)
                {
                    player.TriggerRecoil(holdingItem.itemRef.recoil);
                    player.TriggerSwing(holdingItem.itemRef.swing);

                    Vector3 CurrentPos = pos;

                    for (int i = 0; i < holdingItem.itemRef.fireAmount; i++)
                    {
                        GameManager.SummonProjectile(transform.gameObject,
                            CurrentPos,
                            Quaternion.Euler(0, 0, look + UnityEngine.Random.Range(-holdingItem.itemRef.spread, holdingItem.itemRef.spread)),
                            new ProjectileData(
                                holdingItem.itemRef.bulletSpeed,
                                holdingItem.itemRef.damage,
                                holdingItem.itemRef.bulletLifetime
                            ),
                            holdingItem.itemRef.projectile
                        );
                    }

                    holdingItem.ammo = (holdingItem.ammo > 0) ? holdingItem.ammo - 1 : 0;
                    player.ConsumeMana(holdingItem.itemRef.shooting_manaConsume);

                    if (holdingItem.ammo == 0)
                    {
                        atkCooldown = holdingItem.itemRef.reload;
                        holdingItem.reloading = true;
                    }
                }
                else
                {
                    player.ConsumeMana(holdingItem.itemRef.mele_manaConsume);
                }

                if (holdingItem.itemRef.canMelee)
                {
                    player.TriggerRecoil(holdingItem.itemRef.recoil);
                    player.TriggerSwing(holdingItem.itemRef.swing);

                    Vector3 CurrentPos = pos;

                    if (weaponMuzzle != null) CurrentPos = weaponMuzzle.transform.position;
                    GameManager.SummonAttackArea(
                         CurrentPos,
                         Quaternion.Euler(0, 0, look),
                         new AreaInstance(holdingItem.itemRef.damage, holdingItem.itemRef.hitbox, Game.g_enemies.transform)
                        );
                }

                atkCooldown += holdingItem.itemRef.cooldown;
            }

            if (atkCooldown <= 0f && holdingItem.reloading)
            {
                if (player.ConsumeMana(holdingItem.itemRef.reload_manaConsume))
                {
                    holdingItem.reloading = false;
                    holdingItem.ammo = holdingItem.itemRef.clipSize;
                }
            }
        }
    }

    public void Attack()
    {
        if (holdingItem != null && holdingItem.itemRef)
        {
            Vector2 fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

            if (atkCooldown <= 0f && (holdingItem.itemRef.canShoot || holdingItem.itemRef.canMelee))
            {
                Vector3 CurrentPos = transform.position;
                if (weaponMuzzle != null) CurrentPos = weaponMuzzle.transform.position;

                view.RPC("RPC_Attack", RpcTarget.All, CurrentPos, angle);
            }
        }
    }
}
