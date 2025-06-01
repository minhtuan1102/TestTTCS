using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Đừng quên import SceneManager

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;  // Panel hiển thị khi nhấn ESC
    public Button continueButton;  // Nút tiếp tục game
    public Button exitButton;      // Nút thoát game

    void Start()
    {
        // Ẩn panel khi game bắt đầu
        pausePanel.SetActive(false);

        // Gắn sự kiện cho các nút
        continueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn ESC không
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Nếu panel đang ẩn thì hiển thị, ngược lại thì ẩn đi
            if (!pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(true);  // Hiển thị panel
            }
            else
            {
                pausePanel.SetActive(false); // Ẩn panel
            }
        }
    }

    // Tiếp tục game
    void ContinueGame()
    {
        pausePanel.SetActive(false);  // Ẩn panel
    }

    // Thoát game và quay về scene "StartScene"
    void ExitGame()
    {
        // Quay lại scene "StartScene"
        SceneManager.LoadScene("Start"); 
    }
}
