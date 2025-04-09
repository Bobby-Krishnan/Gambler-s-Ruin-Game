using UnityEngine;
using TMPro;

public class PlayerHealthHUD : MonoBehaviour
{
    // Reference to the player's Health component.
    public Health playerHealth;
    
    // Reference to the TextMeshProUGUI component that displays the health.
    public TextMeshProUGUI healthText;

    void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            // Update the HUD text with the player's current health.
            healthText.text = "Health: " + playerHealth.CurrentHealth;
        }
    }
}
