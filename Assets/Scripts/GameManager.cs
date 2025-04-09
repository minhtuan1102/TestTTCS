using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject gameOverUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ qua các scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ẩn UI Game Over khi bắt đầu game
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("⚠ GameOverUI chưa được gán trong GameManager!");
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
        // Xóa toàn bộ dữ liệu người chơi
        PlayerPrefs.DeleteAll();

        // Load lại màn chơi đầu tiên
        SceneManager.LoadScene("Game"); 
    }

    public void ReturnToMenu()
    {
        // Load lại Main Menu
        SceneManager.LoadScene("Menu"); 
    }
}
