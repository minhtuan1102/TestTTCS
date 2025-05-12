using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] public List<ItemInstance> Items;

    [SerializeField] public float pickUpRange = 2f;

    [SerializeField] public ItemInstance holdingItem;

    [SerializeField] public List<ItemInstance> Armor;

    private Transform nearestItem;

    private float scanTime = 0.2f;
    private float scanTimer = 0f;

    private string currentWeaponModel = "";
    private Transform weaponMuzzle;

    private Player player;
    private PhotonView view;

    // Attack

    private float timer = 0f;
    private float atkCooldown = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        view = GetComponent<PhotonView>();
    }

    public void Holding(ItemInstance item)
    {
        if (Items.Contains(item))
        {
            holdingItem = item;
        }
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


    // Server

    public void AddItem(ItemInstance item)
    {
        int index = FindItem(item);
        if (index>=0 && item.itemRef.isConsumable)
        {
            Items[index].amount += item.amount;
        } else
        {
            Items.Add(new ItemInstance(item));
        }
        PlayerUI.UI.GetComponent<PlayerUI>().UpdateInventory();
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
                GameManager.Instance.SpawnItem(Items[index].itemRef, DropAmount, transform.position, new Quaternion(0, 0, 0, 0));

                Items[index].amount -= DropAmount;
                if (Items[index].amount <= 0)
                {
                    Items.RemoveAt(index);
                }
            }
            else
            {
                GameManager.Instance.SpawnItem(Items[index], transform.position, new Quaternion(0, 0, 0, 0));
                Items.RemoveAt(index);
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
                if (Items[index].amount <= 0)
                {
                    Items.RemoveAt(index);
                }
                PlayerUI.UI.transform.GetComponent<PlayerUI>().UpdateInventory();
            }
        }
    }

    // Local

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
                    AddItem(nearestItem.GetComponent<ItemObject>().Data);
                    Destroy(nearestItem.gameObject);
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
        }
    }

    public void Attack()
    {
        if (holdingItem != null && holdingItem.itemRef)
        {
            Vector2 fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            if (atkCooldown <= 0f && (holdingItem.ammo>0 && holdingItem.itemRef.canShoot || holdingItem.itemRef.canMelee))
            {
                player.TriggerRecoil(holdingItem.itemRef.recoil);
                player.TriggerSwing(holdingItem.itemRef.swing);

                if (holdingItem.itemRef.canShoot)
                {

                    Vector3 CurrentPos = transform.position;

                    if (weaponMuzzle != null) CurrentPos = weaponMuzzle.transform.position;

                    for (int i = 0; i < holdingItem.itemRef.fireAmount; i++)
                    {
                        GameManager.SummonProjectile(transform.gameObject,
                            CurrentPos,
                            Quaternion.Euler(0, 0, player.lookDir + UnityEngine.Random.Range(-holdingItem.itemRef.spread, holdingItem.itemRef.spread)),
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
                    Vector3 CurrentPos = transform.position;

                    if (weaponMuzzle != null) CurrentPos = weaponMuzzle.transform.position;
                    GameManager.SummonAttackArea(
                         CurrentPos,
                         Quaternion.Euler(0, 0, player.lookDir),
                         new AreaInstance(holdingItem.itemRef.damage, holdingItem.itemRef.hitbox, Game.g_enemies.transform)
                        );
                }

                FireBullet Attack_Shoot = GetComponent<FireBullet>();
                if (Attack_Shoot != null)
                {
                    Attack_Shoot.Shoot(holdingItem.itemRef.damage, holdingItem.itemRef.spread, holdingItem.itemRef.fireAmount);
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
}
