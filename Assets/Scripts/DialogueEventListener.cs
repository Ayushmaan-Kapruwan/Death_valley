using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Listens for dialogue events and triggers appropriate dialogues
/// </summary>
public class DialogueEventListener : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Character Dialogues")]
    [SerializeField] private List<CharacterDialogueConfig> characterConfigs = new List<CharacterDialogueConfig>();

    [Header("Settings")]
    [SerializeField] private bool queueDialogues = true;
    [Tooltip("If true, new dialogues interrupt current ones")]
    [SerializeField] private bool allowInterruption = false;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private HashSet<string> triggeredOnceEvents = new HashSet<string>();
    private Queue<System.Action> dialogueQueue = new Queue<System.Action>();
    private bool isShowingDialogue = false;

    private void OnEnable()
    {
        DialogueEventManager.OnDialogueEventTriggered += HandleDialogueEvent;
    }

    private void OnDisable()
    {
        DialogueEventManager.OnDialogueEventTriggered -= HandleDialogueEvent;
    }

    private void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            
            if (dialogueManager == null)
            {
                Debug.LogError("[DialogueEventListener] DialogueManager not found in scene!");
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueEventListener] Initialized with {characterConfigs.Count} character config(s)");
            foreach (var config in characterConfigs)
            {
                if (config != null)
                {
                    Debug.Log($"  - Config: {config.characterName} with {config.eventDialogues.Count} event dialogue(s)");
                    foreach (var eventDialogue in config.eventDialogues)
                    {
                        Debug.Log($"    • Event Type: {eventDialogue.eventType}, Custom Name: '{eventDialogue.customEventName}'");
                    }
                }
                else
                {
                    Debug.LogWarning("[DialogueEventListener] Null config found in characterConfigs list!");
                }
            }
        }
    }

    private void HandleDialogueEvent(DialogueEventData eventData)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueEventListener] Received event: {eventData.eventType} (Custom: '{eventData.customEventName}')");
        }
        
        // Find all matching dialogues from all characters
        List<CharacterDialogueConfig.EventDialogue> matches = new List<CharacterDialogueConfig.EventDialogue>();

        foreach (var config in characterConfigs)
        {
            if (config == null) continue;
            
            foreach (var eventDialogue in config.eventDialogues)
            {
                if (IsEventMatch(eventDialogue, eventData))
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[DialogueEventListener] Found match in config '{config.characterName}'!");
                    }
                    
                    // Check if already triggered once
                    string key = $"{config.characterName}_{eventData.eventType}_{eventDialogue.customEventName}";
                    if (eventDialogue.triggerOnce && triggeredOnceEvents.Contains(key))
                    {
                        if (enableDebugLogs)
                        {
                            Debug.Log($"[DialogueEventListener] Skipping - already triggered once (key: {key})");
                        }
                        continue;
                    }

                    matches.Add(eventDialogue);
                    
                    if (eventDialogue.triggerOnce)
                    {
                        triggeredOnceEvents.Add(key);
                    }
                }
            }
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueEventListener] Total matches found: {matches.Count}");
        }
        
        if (matches.Count == 0)
        {
            Debug.LogWarning($"[DialogueEventListener] No matching dialogue found for event: {eventData.eventType} ('{eventData.customEventName}')");
            return;
        }

        // Sort by priority (highest first)
        matches.Sort((a, b) => b.priority.CompareTo(a.priority));

        // Trigger dialogues
        foreach (var match in matches)
        {
            if (match.dialogue == null)
            {
                Debug.LogError("[DialogueEventListener] Dialogue reference is null!");
                continue;
            }
            
            if (match.delay > 0)
            {
                StartCoroutine(TriggerDialogueWithDelay(match.dialogue, match.delay));
            }
            else
            {
                TriggerDialogue(match.dialogue);
            }
        }
    }

    private bool IsEventMatch(CharacterDialogueConfig.EventDialogue eventDialogue, DialogueEventData eventData)
    {
        // First check: event types must match
        if (eventDialogue.eventType != eventData.eventType)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[DialogueEventListener] Event type mismatch: {eventDialogue.eventType} != {eventData.eventType}");
            }
            return false;
        }

        // Second check: for Custom events or events with custom names, compare custom names
        if (eventData.eventType == DialogueEventType.Custom || !string.IsNullOrEmpty(eventData.customEventName))
        {
            bool match = eventDialogue.customEventName == eventData.customEventName;
            if (enableDebugLogs && !match)
            {
                Debug.Log($"[DialogueEventListener] Custom name mismatch: '{eventDialogue.customEventName}' != '{eventData.customEventName}'");
            }
            return match;
        }

        // If event type matches and no custom name to check, it's a match
        return true;
    }

    private void TriggerDialogue(Dialogue dialogue)
    {
        if (dialogueManager == null)
        {
            Debug.LogError("[DialogueEventListener] DialogueManager reference is missing!");
            return;
        }
        
        if (dialogue == null)
        {
            Debug.LogError("[DialogueEventListener] Dialogue is null!");
            return;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueEventListener] Triggering dialogue: {dialogue.name}");
        }

        if (queueDialogues && isShowingDialogue && !allowInterruption)
        {
            if (enableDebugLogs)
            {
                Debug.Log("[DialogueEventListener] Dialogue already active - queueing this one");
            }
            dialogueQueue.Enqueue(() => dialogueManager.StartDialogue(dialogue));
        }
        else
        {
            if (allowInterruption)
            {
                StopAllCoroutines();
            }
            
            isShowingDialogue = true;
            dialogueManager.StartDialogue(dialogue);
            StartCoroutine(WaitForDialogueEnd());
        }
    }

    private IEnumerator TriggerDialogueWithDelay(Dialogue dialogue, float delay)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueEventListener] Delaying dialogue by {delay} seconds");
        }
        
        yield return new WaitForSeconds(delay);
        TriggerDialogue(dialogue);
    }

    private IEnumerator WaitForDialogueEnd()
    {
        // Wait while dialogue is active
        while (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Process next dialogue in queue
        if (dialogueQueue.Count > 0)
        {
            var nextDialogue = dialogueQueue.Dequeue();
            nextDialogue?.Invoke();
        }
        else
        {
            isShowingDialogue = false;
        }
    }

    /// <summary>
    /// Call this from DialogueManager when dialogue ends
    /// </summary>
    public void OnDialogueEnded()
    {
        isShowingDialogue = false;
        
        // Process queued dialogues
        if (dialogueQueue.Count > 0)
        {
            var nextDialogue = dialogueQueue.Dequeue();
            nextDialogue?.Invoke();
        }
    }
}
