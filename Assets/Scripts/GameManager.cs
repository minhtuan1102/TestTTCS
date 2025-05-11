using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverUI;

    private void Awake()
    {
        // Khởi tạo Singleton tạm thời, không dùng DontDestroyOnLoad để tránh lỗi UI
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Đảm bảo ẩn UI khi bắt đầu
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Game"); // Đổi thành tên scene gốc
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu"); // Đổi thành tên scene menu
    }

    // Game Engine

    public static void SummonAttackArea(Vector3 pos, Quaternion dir, AreaInstance data)
    {
        GameObject area = Instantiate(Game.AreaAtkSample, pos, dir, Game.g_projectiles.transform);
        AreaAttack areaAttack = area.GetComponent<AreaAttack>();
        areaAttack.data = data;

        area.SetActive(true);

        areaAttack.Attack();
    }

    // Player Interaction

    public static void SummonProjectile(GameObject player, Vector3 pos, Quaternion dir, ProjectileData data, GameObject model)
    {
        GameObject bullet = Instantiate(model, pos, dir, Game.g_projectiles.transform);
        Projectile bullet_Projectile = bullet.GetComponent<Projectile>();
        bullet_Projectile.itemData = data;

        bullet.SetActive(true);
    }

    // Command

    public static void SetHealth(GameObject target, float health)
    {
        Player player = target.transform.GetComponent<Player>();
        player.SetHealth(health);
    }

    public static void SetMana(GameObject target, float mana)
    {
        Player player = target.transform.GetComponent<Player>();
        player._currentMana = Mathf.Min(player.MaxMana, mana);
    }

    public static void SpawnItem(string name, int amount, Vector3 pos, Quaternion rot) 
    {
        Item itemRef = Game.GetItemDataFromName(name);
        //Debug.Log(Game.ItemObjectSample);
        //Debug.Log(name);
        //Debug.Log(amount);
        if (itemRef)
        {
            GameObject newItem = Instantiate(Game.ItemObjectSample, pos, rot, Game.g_items.transform);
            ItemObject data = newItem.GetComponent<ItemObject>();
            data.Data.itemRef = itemRef;
            data.Data.amount = amount;
            data.Data.ammo = itemRef.clipSize;

            newItem.SetActive(true);
        }
    }

    public static void SpawnItem(Item itemRef, int amount, Vector3 pos, Quaternion rot)
    {
        if (itemRef != null)
        {
            GameObject newItem = Instantiate(Game.ItemObjectSample, pos, rot, Game.g_items.transform);
            ItemObject data = newItem.GetComponent<ItemObject>();
            data.Data.itemRef = itemRef;
            data.Data.amount = amount;
            data.Data.ammo = itemRef.clipSize;

            newItem.SetActive(true);
        }
    }

    public static void SpawnItem(ItemInstance dat, Vector3 pos, Quaternion rot)
    {
        if (dat != null)
        {
            GameObject newItem = Instantiate(Game.ItemObjectSample, pos, rot, Game.g_items.transform);
            ItemObject data = newItem.GetComponent<ItemObject>();
            data.Data = new ItemInstance(dat);

            newItem.SetActive(true);
        }
    }
}
