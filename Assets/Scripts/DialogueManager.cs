using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public string text;
    public float duration = 3f; // How long to display this line
}

[System.Serializable]
public class Dialogue
{
    public string dialogueID;
    public List<DialogueLine> lines = new List<DialogueLine>();
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUIPanel;
    [SerializeField] private UnityEngine.UI.Text characterNameText;
    [SerializeField] private UnityEngine.UI.Text dialogueText;
    [SerializeField] private float textDisplaySpeed = 0.05f; // Speed of text reveal animation
    
    private List<Dialogue> dialogues = new List<Dialogue>();
    private Dialogue currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private Coroutine textRevealCoroutine;
    
    private static DialogueManager instance;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (dialogueUIPanel != null)
            dialogueUIPanel.SetActive(false);
    }

    void Update()
    {
        // Press SPACE to advance dialogue
        if (isDialogueActive && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceDialogue();
        }
    }

    /// <summary>
    /// Start a dialogue by ID
    /// </summary>
    public void StartDialogue(string dialogueID)
    {
        Dialogue dialogue = dialogues.Find(d => d.dialogueID == dialogueID);
        
        if (dialogue == null)
        {
            Debug.LogWarning($"Dialogue with ID '{dialogueID}' not found!");
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        
        if (dialogueUIPanel != null)
            dialogueUIPanel.SetActive(true);
        
        DisplayCurrentLine();
    }

    /// <summary>
    /// Display the current line of dialogue
    /// </summary>
    void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        
        if (characterNameText != null)
            characterNameText.text = line.characterName;
        
        // Stop previous text reveal if any
        if (textRevealCoroutine != null)
            StopCoroutine(textRevealCoroutine);
        
        // Start text reveal animation
        textRevealCoroutine = StartCoroutine(RevealTextCoroutine(line.text, line.duration));
    }

    /// <summary>
    /// Coroutine to reveal text character by character
    /// </summary>
    IEnumerator RevealTextCoroutine(string fullText, float duration)
    {
        dialogueText.text = "";
        int charIndex = 0;

        while (charIndex < fullText.Length)
        {
            dialogueText.text += fullText[charIndex];
            charIndex++;
            yield return new WaitForSeconds(textDisplaySpeed);
        }

        // Wait for duration before allowing next line
        yield return new WaitForSeconds(duration - (fullText.Length * textDisplaySpeed));
    }

    /// <summary>
    /// Advance to the next dialogue line
    /// </summary>
    public void AdvanceDialogue()
    {
        if (!isDialogueActive || currentDialogue == null)
            return;

        currentLineIndex++;
        DisplayCurrentLine();
    }

    /// <summary>
    /// End the current dialogue
    /// </summary>
    public void EndDialogue()
    {
        isDialogueActive = false;
        currentDialogue = null;
        currentLineIndex = 0;

        if (dialogueUIPanel != null)
            dialogueUIPanel.SetActive(false);

        if (textRevealCoroutine != null)
        {
            StopCoroutine(textRevealCoroutine);
            textRevealCoroutine = null;
        }

        Debug.Log("Dialogue ended");
    }

    /// <summary>
    /// Register a dialogue to the manager
    /// </summary>
    public void RegisterDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("Attempted to register null dialogue!");
            return;
        }

        dialogues.Add(dialogue);
    }

    /// <summary>
    /// Check if dialogue is currently playing
    /// </summary>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static DialogueManager Instance
    {
        get { return instance; }
    }
}
