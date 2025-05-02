using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;                  // Dialogue panel
    public TextMeshProUGUI dialogueText;              // Dialogue text display
    public TextMeshProUGUI continuePrompt;            // "Press Enter to continue"
    public string[] lines;                          // Lines of dialogue 
    public float typingSpeed = 0.05f;

    public TutorialHint tutorialHint;               
    public System.Action OnDialogueComplete;        // Callback after dialogue ends

    private int index = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;

    void Start()
    {
        // Set initial state on scene reload:
        dialogueActive = false;
        dialogueBox.SetActive(false);
        continuePrompt.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = lines[index];
                isTyping = false;
                continuePrompt.gameObject.SetActive(true);
            }
            else
            {
                continuePrompt.gameObject.SetActive(false);
                index++;

                if (index < lines.Length)
                {
                    StartCoroutine(TypeLine());
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    // Call this method to start a dialogue sequence.
    public void StartDialogue(string[] newLines)
    {
        Time.timeScale = 0f; // Pause the game for dialogue.
        dialogueActive = true;
        lines = newLines;
        index = 0;
        dialogueBox.SetActive(true);
        continuePrompt.gameObject.SetActive(false);
        StartCoroutine(TypeLine());
    }

    // Ends the dialogue and resumes gameplay.
    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        Time.timeScale = 1f; // Resume normal time.
        dialogueActive = false;

        if (tutorialHint != null)
        {
            tutorialHint.ShowHint("Press Z to throw your hat.");
        }

        OnDialogueComplete?.Invoke();
    }

    // Types out the current dialogue line letter by letter.
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";
        continuePrompt.gameObject.SetActive(false);

        foreach (char c in lines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        continuePrompt.gameObject.SetActive(true);
    }
}
