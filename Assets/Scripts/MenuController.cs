using UnityEngine;
using UnityEngine.SceneManagement;

// Load scenes when player starts the game
public class MainMenuController : MonoBehaviour
{

    public string levelSceneName = "Level1";

    public void PlayGame()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

}
