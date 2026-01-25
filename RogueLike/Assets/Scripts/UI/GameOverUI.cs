using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Inspector: UI
    [Header("UI")]
    [Tooltip("Root Game Over panel. Should start disabled.")]
    public GameObject gameOverPanel;

    [Tooltip("If true, pauses the game when Game Over is shown.")]
    public bool pauseOnShow = true;

    // Unity lifecycle
    private void Awake()
    {
        // Ensure Game Over UI is hidden at the start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // Public API (called by PlayerHealth.onDeath)
    public void Show()
    {
        // Show Game Over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Pause gameplay
        if (pauseOnShow)
            Time.timeScale = 0f;
    }

    // Button callback
    public void Restart()
    {
        // Resume time and reload current scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Button callback
    public void Quit()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
