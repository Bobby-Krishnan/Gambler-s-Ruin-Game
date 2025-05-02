using UnityEngine;

public class FinalBossTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    [TextArea(2, 5)]
    public string[] bossDialogueLines;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (dialogueManager != null && bossDialogueLines.Length > 0)
            {
                dialogueManager.StartDialogue(bossDialogueLines);
            }

            gameObject.SetActive(false); // disable trigger after use
        }
    }
}
