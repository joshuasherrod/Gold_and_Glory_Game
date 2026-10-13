using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // UI windows used during the game
    public GameObject pauseWindow;
    public GameObject gameOverWindow;
    public GameObject victoryWindow;

    public string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    // Start game
    void Start()
    {
    Time.timeScale = 1f;
    isPaused = false;

    pauseWindow.SetActive(false);
    gameOverWindow.SetActive(false);
    victoryWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Resume game and hide pause window
    public void PauseGame()
    {
        isPaused = true;

        // Display pause menu
        pauseWindow.SetActive(true);
        // Stop game
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Hide pause menu
        pauseWindow.SetActive(false);
        // Continue game
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        // Reset time before loading scene
        Time.timeScale = 1f;

        // Reload currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        isPaused = true;
        // Game over screen
        gameOverWindow.SetActive(true);
        // Pause game
        Time.timeScale = 0f;
    }

    public void LevelComplete()
    {

        isPaused = true;
        // victory screen
        victoryWindow.SetActive(true);
        // Pause game
        Time.timeScale = 0f;
    }

    public void ReturnToMainMenu()
    {
        // Reset time before changing scenes
        Time.timeScale = 1f;
        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}


