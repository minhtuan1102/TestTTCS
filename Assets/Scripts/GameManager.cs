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

    // Player Interaction

    public static void PlayerPickup(GameObject player, GameObject item)
    {
        PlayerInventory pInventory = player.GetComponent<PlayerInventory>();


    }

    // Command

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


        }
    }
}
