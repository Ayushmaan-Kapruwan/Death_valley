using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.05f; // Adjustable in Inspector (0.01 = 10ms per character)

    private Queue<string> sentences;
    private bool dialogueActive = false;
    
    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        // Press E to advance to next sentence while dialogue is active
        if (dialogueActive && Input.GetKeyDown(KeyCode.Mouse0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);
        
        nameText.text = dialogue.name;

        sentences.Clear();

        // Check if sentences exist
        if (dialogue.sentences == null || dialogue.sentences.Length == 0)
        {
            Debug.LogError("No sentences found in dialogue!");
            return;
        }

        Debug.Log("Number of sentences: " + dialogue.sentences.Length);

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        dialogueActive = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // Stop any ongoing typing effect
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) // ToCharArray() converts a string into a character array
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait before showing next character
        }
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation.");
        dialogueActive = false;

        animator.SetBool("IsOpen", false);
    }
}
