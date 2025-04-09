using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Player Settings")]
    public bool isPlayer = false;

    
    public GameObject gameContent;  
    public GameObject startScreen;  

    void Start()
    {
        currentHealth = maxHealth;
        Time.timeScale = 1f;
    }

    // read the current health.
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took damage! Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Player died. Returning to main menu.");
            Time.timeScale = 1f;
            // Load the Main Menu scene.
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            EnemyAI enemy = GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.Die();
        }
    }
}
