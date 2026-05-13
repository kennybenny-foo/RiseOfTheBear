using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject hud;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    [Header("Exit")]
    [SerializeField] LevelExit levelExit;

    int enemiesRemaining;
    bool gameEnded = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (hud != null)
        {
            hud.SetActive(true);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CountEnemies();

        if (levelExit != null)
        {
            levelExit.SetLocked(enemiesRemaining > 0);
        }

        Debug.Log("Enemies remaining: " + enemiesRemaining);
    }

    void CountEnemies()
    {
        Health[] enemyHealthObjects = FindObjectsByType<Health>(FindObjectsSortMode.None);
        enemiesRemaining = enemyHealthObjects.Length;
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;

        if (enemiesRemaining < 0)
        {
            enemiesRemaining = 0;
        }

        Debug.Log("Enemies remaining: " + enemiesRemaining);

        if (enemiesRemaining <= 0 && levelExit != null)
        {
            levelExit.SetLocked(false);
            Debug.Log("Prize chute exit unlocked!");
        }
    }

    public void WinGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        if (hud != null)
        {
            hud.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        EndGamePause();
    }

    public void LoseGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        if (hud != null)
        {
            hud.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        EndGamePause();
    }

    void EndGamePause()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}