using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages natural conversation flow between characters (supports multiple consecutive lines per character)
/// </summary>
public class TurnBasedDialogueController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [Tooltip("The dialogue sentences for this line")]
        [TextArea(3, 5)]
        public string[] sentences;
        [Tooltip("Delay before this line starts (in seconds)")]
        public float delayBeforeLine = 0.5f;
    }
    
    [Header("Conversation Setup")]
    [SerializeField] private List<DialogueLine> conversationLines = new List<DialogueLine>();
    [Tooltip("Automatically start conversation after this delay")]
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private bool autoStartOnSceneLoad = true;
    
    [Header("Scene Transition")]
    [SerializeField] private bool loadSceneAfterConversation = true;
    [SerializeField] private string sceneToLoad = "MainGame";
    
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private int currentLineIndex = 0;
    private bool conversationActive = false;
    private bool conversationStarted = false;
    
    private void Start()
    {
        // Find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            
            if (dialogueManager == null)
            {
                Debug.LogError("[TurnBasedDialogueController] DialogueManager not found in scene!");
                return;
            }
        }
        
        // Validate conversation setup
        if (conversationLines.Count == 0)
        {
            Debug.LogError("[TurnBasedDialogueController] No conversation lines configured!");
            return;
        }
        
        // Auto-start if enabled
        if (autoStartOnSceneLoad)
        {
            StartCoroutine(StartConversationWithDelay());
        }
    }
    
    private IEnumerator StartConversationWithDelay()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[TurnBasedDialogueController] Waiting {initialDelay} seconds before starting conversation...");
        }
        
        yield return new WaitForSeconds(initialDelay);
        StartConversation();
    }
    
    /// <summary>
    /// Start the conversation
    /// </summary>
    public void StartConversation()
    {
        if (conversationStarted)
        {
            Debug.LogWarning("[TurnBasedDialogueController] Conversation already started!");
            return;
        }
        
        if (conversationLines.Count == 0)
        {
            Debug.LogError("[TurnBasedDialogueController] No conversation lines configured!");
            return;
        }
        
        conversationStarted = true;
        currentLineIndex = 0;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[TurnBasedDialogueController] Starting conversation with {conversationLines.Count} lines");
        }
        
        StartCoroutine(PlayNextLine());
    }
    
    private IEnumerator PlayNextLine()
    {
        if (currentLineIndex >= conversationLines.Count)
        {
            // Conversation finished
            OnConversationComplete();
            yield break;
        }
        
        DialogueLine currentLine = conversationLines[currentLineIndex];
        
        if (currentLine.sentences == null || currentLine.sentences.Length == 0)
        {
            Debug.LogError($"[TurnBasedDialogueController] No sentences for line {currentLineIndex}!");
            currentLineIndex++;
            StartCoroutine(PlayNextLine());
            yield break;
        }
        
        // Wait for delay before this line
        if (currentLine.delayBeforeLine > 0)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[TurnBasedDialogueController] Waiting {currentLine.delayBeforeLine}s before {currentLine.characterName} speaks...");
            }
            yield return new WaitForSeconds(currentLine.delayBeforeLine);
        }
        
        // Create a temporary Dialogue object for this line
        Dialogue tempDialogue = ScriptableObject.CreateInstance<Dialogue>();
        tempDialogue.name = currentLine.characterName;
        tempDialogue.sentences = currentLine.sentences;
        
        // Start dialogue for this line
        if (enableDebugLogs)
        {
            Debug.Log($"[TurnBasedDialogueController] Line {currentLineIndex + 1}/{conversationLines.Count}: {currentLine.characterName} ({currentLine.sentences.Length} sentences)");
        }
        
        conversationActive = true;
        dialogueManager.StartDialogue(tempDialogue);
        
        // Wait for dialogue to complete
        StartCoroutine(WaitForDialogueComplete());
    }
    
    private IEnumerator WaitForDialogueComplete()
    {
        // Wait a frame to ensure dialogue has started
        yield return new WaitForSeconds(0.1f);
        
        // Wait while dialogue is active
        while (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        conversationActive = false;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[TurnBasedDialogueController] Line {currentLineIndex + 1} completed");
        }
        
        // Move to next line
        currentLineIndex++;
        StartCoroutine(PlayNextLine());
    }
    
    private void OnConversationComplete()
    {
        if (enableDebugLogs)
        {
            Debug.Log("[TurnBasedDialogueController] Conversation complete!");
        }
        
        conversationActive = false;
        conversationStarted = false;
        
        // Load scene if configured
        if (loadSceneAfterConversation)
        {
            StartCoroutine(LoadSceneAfterDelay());
        }
    }
    
    private IEnumerator LoadSceneAfterDelay()
    {
        // Brief delay before scene transition
        yield return new WaitForSeconds(0.5f);
        
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("[TurnBasedDialogueController] Scene name is not set!");
            yield break;
        }
        
        // Check if scene exists in build settings
        if (!SceneExistsInBuildSettings(sceneToLoad))
        {
            Debug.LogError($"[TurnBasedDialogueController] Scene '{sceneToLoad}' not found in Build Settings!");
            yield break;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[TurnBasedDialogueController] Loading scene: {sceneToLoad}");
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
    
    private bool SceneExistsInBuildSettings(string sceneName)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Skip to next line (useful for testing or skip button)
    /// </summary>
    public void SkipToNextLine()
    {
        if (!conversationActive) return;
        
        StopAllCoroutines();
        currentLineIndex++;
        StartCoroutine(PlayNextLine());
    }
    
    /// <summary>
    /// Restart the conversation from the beginning
    /// </summary>
    public void RestartConversation()
    {
        StopAllCoroutines();
        conversationStarted = false;
        conversationActive = false;
        currentLineIndex = 0;
        StartConversation();
    }
}