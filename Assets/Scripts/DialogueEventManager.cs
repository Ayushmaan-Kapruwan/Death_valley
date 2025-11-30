using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum defining different types of dialogue events
/// </summary>
public enum DialogueEventType
{ 
    OnCheckpointReached,
    OnClueFound,
    OnInteraction,
    Custom
}

/// <summary>
/// Event data for triggering dialogues
/// </summary>
public class DialogueEventData
{
    public DialogueEventType eventType;
    public string customEventName;
    public object additionalData;

    public DialogueEventData(DialogueEventType type, string customName = "", object data = null)
    {
        eventType = type;
        customEventName = customName;
        additionalData = data;
    }
}

/// <summary>
/// Centralized manager for dialogue events
/// </summary>
public class DialogueEventManager : MonoBehaviour
{
    public static DialogueEventManager Instance { get; private set; }

    // Event delegates
    public static event Action<DialogueEventData> OnDialogueEventTriggered;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Trigger a dialogue event
    /// </summary>
    public void TriggerEvent(DialogueEventType eventType, string customName = "", object data = null)
    {
        var eventData = new DialogueEventData(eventType, customName, data);
        
        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueEventManager] Triggering event: {eventType} (Custom: {customName})");
        }

        OnDialogueEventTriggered?.Invoke(eventData);
    }

    /// <summary>
    /// Trigger a custom named event
    /// </summary>
    public void TriggerCustomEvent(string customName, object data = null)
    {
        TriggerEvent(DialogueEventType.Custom, customName, data);
    }
}