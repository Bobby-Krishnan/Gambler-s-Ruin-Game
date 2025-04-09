using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneGate : MonoBehaviour
{
    [Header("Zone Settings")]
    public Transform enemyContainer;
    public DialogueManager dialogueManager;
    [TextArea(2, 5)]
    public string[] postDialogueLines;

    [Header("Optional Player Unlocks")]
    public GideonShoot gideonShoot;

    [Header("Final Zone Settings")]
    // Set this to true in the Inspector for the zone that contains the final boss.
    public bool isFinalZone = false;

    private bool gateOpened = false;
    private bool gateActive = false; // Flag to allow checking when ready

    void Start()
    {
        // Automatically activate gate checking when the scene loads.
        ActivateGateCheck();
    }

    void Update()
    {
        if (!gateOpened && gateActive)
        {
            CheckIfEnemiesCleared();
        }
    }

    public void CheckIfEnemiesCleared()
    {
        if (enemyContainer == null)
            return;

        int remaining = 0;
        // Loop through enemy container children and count those still active and not dead.
        foreach (Transform child in enemyContainer)
        {
            if (child.gameObject.activeInHierarchy)
            {
                EnemyAI enemy = child.GetComponent<EnemyAI>();
                if (enemy == null || !enemy.isDead)
                {
                    remaining++;
                }
            }
        }

        // Debug: Log remaining count
        Debug.Log("ZoneGate: " + remaining + " enemies remaining in zone.");

        // If no active enemy remains, open the gate.
        if (remaining == 0)
        {
            OpenGate();
        }
    }

    void OpenGate()
    {
        gateOpened = true;

        if (dialogueManager != null && postDialogueLines.Length > 0)
        {
            if (isFinalZone)
            {
                Debug.Log("Final zone cleared, starting final dialogue.");
                // For the final zone, after dialogue, load the Main Menu scene.
                dialogueManager.OnDialogueComplete = () =>
                {
                    Debug.Log("Final dialogue complete callback invoked.");
                    StartCoroutine(WaitAndLoadStartMenu());
                };
            }
            else
            {
                // For normal zones, you might unlock abilities.
                dialogueManager.OnDialogueComplete = () =>
                {
                    if (gideonShoot != null)
                        gideonShoot.hasGun = true;
                };
            }

            // Start the dialogue that plays when the zone is cleared.
            dialogueManager.StartDialogue(postDialogueLines);
        }

        // Disable the gate (or hide the barrier).
        gameObject.SetActive(false);
    }

    IEnumerator WaitAndLoadStartMenu()
    {
        // Wait for a few seconds after the dialogue finishes.
        yield return new WaitForSeconds(3f); // Adjust the time as desired.
        Debug.Log("ZoneGate: Loading MainMenuScene...");
        SceneManager.LoadScene("MainMenuScene");
    }

    // A method to manually reset the gate during gameplay.
    public void ResetGate()
    {
        gateOpened = false;
        gateActive = false;
        gameObject.SetActive(true);

        if (dialogueManager != null)
            dialogueManager.OnDialogueComplete = null;

        if (gideonShoot != null)
            gideonShoot.hasGun = false;
    }

    // Call this method to activate the enemy-checking process.
    public void ActivateGateCheck()
    {
        gateActive = true;
    }
}
