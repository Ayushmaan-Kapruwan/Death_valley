using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the TruckScene flow: triggers dialogue after delay and loads MainGame scene when dialogue ends
/// </summary>
public class TruckSceneController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainGameSceneName = "MainGame";
    [Tooltip("Scene to load after dialogue ends")]
    
    [Header("Dialogue Settings")]
    [SerializeField] private float dialogueStartDelay = 0.5f;
    [SerializeField] private string customEventName = "TruckSceneIntro";
    [Tooltip("Custom event name to trigger the dialogue")]
    
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private bool dialogueTriggered = false;
    private bool isWaitingForDialogue = false;

    private void Start()
    {
        // Find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            
            if (dialogueManager == null)
            {
                Debug.LogError("[TruckSceneController] DialogueManager not found in scene!");
                return;
            }
        }
        
        // Start the dialogue trigger sequence
        StartCoroutine(TriggerDialogueSequence());
    }

    private IEnumerator TriggerDialogueSequence()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[TruckSceneController] Waiting {dialogueStartDelay} seconds before triggering dialogue...");
        }
        
        // Wait for the specified delay
        yield return new WaitForSeconds(dialogueStartDelay);
        
        // Trigger the dialogue event
        if (DialogueEventManager.Instance != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[TruckSceneController] Triggering custom event: {customEventName}");
            }
            
            DialogueEventManager.Instance.TriggerCustomEvent(customEventName);
            dialogueTriggered = true;
            isWaitingForDialogue = true;
            
            // Start waiting for dialogue to end
            StartCoroutine(WaitForDialogueEnd());
        }
        else
        {
            Debug.LogError("[TruckSceneController] DialogueEventManager.Instance is null!");
        }
    }

    private IEnumerator WaitForDialogueEnd()
    {
        if (enableDebugLogs)
        {
            Debug.Log("[TruckSceneController] Waiting for dialogue to end...");
        }
        
        // Wait a frame to ensure dialogue has started
        yield return new WaitForSeconds(0.2f);
        
        // Wait while dialogue is active
        while (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[TruckSceneController] Dialogue ended. Loading scene: {mainGameSceneName}");
        }
        
        isWaitingForDialogue = false;
        
        // Load the MainGame scene
        LoadMainGameScene();
    }

    private void LoadMainGameScene()
    {
        if (string.IsNullOrEmpty(mainGameSceneName))
        {
            Debug.LogError("[TruckSceneController] Main game scene name is not set!");
            return;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[TruckSceneController] Loading scene: {mainGameSceneName}");
        }
        
        SceneManager.LoadScene(mainGameSceneName);
    }
}