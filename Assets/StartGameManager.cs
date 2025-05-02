using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : MonoBehaviour
{
    [Header("Gameplay Scene")]
    // In the Inspector, type in: MainGameScene
    public string gameplaySceneName;

    void Start()
    {
        // Make sure time is running normally when this scene starts.
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        // In case time was paused elsewhere, reset it.
        Time.timeScale = 1f;
        // Load the gameplay scene by its name.
        SceneManager.LoadScene(gameplaySceneName);
    }
}
