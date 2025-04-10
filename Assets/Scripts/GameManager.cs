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
}
