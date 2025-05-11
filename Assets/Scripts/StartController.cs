using UnityEngine;
using UnityEngine.SceneManagement;  // Để sử dụng chức năng chuyển scene
using UnityEngine.UI;  // Để xử lý UI

public class StartController : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Menu");
    }
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void OpenOptions()
    {
        Debug.Log("Options menu chưa được thiết lập.");
    }
}
