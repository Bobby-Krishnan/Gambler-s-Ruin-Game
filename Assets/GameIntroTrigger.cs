using System.Collections;
using UnityEngine;

public class GameIntroTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    // Set up intro lines in the Inspector.
    [TextArea(2, 5)]
    public string[] introLines;

    void Start()
    {
        // Start the coroutine to delay dialogue start.
        StartCoroutine(StartDialogueAfterDelay());
    }

    IEnumerator StartDialogueAfterDelay()
    {
        // Wait until the end of the frame so DialogueManager.Start() has run.
        yield return new WaitForEndOfFrame();
        
        dialogueManager.StartDialogue(introLines);
    }
}
