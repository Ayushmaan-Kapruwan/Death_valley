using System.Collections;
using UnityEngine;

/// <summary>
/// Triggers dialogue events when player enters a specific area
/// </summary>
[RequireComponent(typeof(Collider))]
public class DialogueAreaTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Name of the player tag")]
    [SerializeField] private string playerTag = "Player";
    
    [Tooltip("The event name to trigger (used for matching in CharacterDialogueConfig)")]
    [SerializeField] private string eventName = "AreaEntered";
    
    [Tooltip("Type of dialogue event to trigger")]
    [SerializeField] private DialogueEventType eventType = DialogueEventType.Custom;
    
    [Header("Behavior")]
    [Tooltip("Should this trigger only activate once?")]
    [SerializeField] private bool triggerOnce = true;
    
    [Tooltip("Delay before triggering dialogue (in seconds)")]
    [SerializeField] private float dialogueDelay = 0.5f;
    
    [Tooltip("Should trigger activate on enter or exit?")]
    [SerializeField] private TriggerMode triggerMode = TriggerMode.OnEnter;
    
    [Header("Visual Debug")]
    [Tooltip("Show the trigger area in Scene view")]
    [SerializeField] private bool showGizmo = true;
    
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);
    
    private bool hasTriggered = false;
    private Collider triggerCollider;
    
    public enum TriggerMode
    {
        OnEnter,
        OnExit,
        Both
    }
    
    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        
        // Ensure collider is set as trigger
        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning($"[DialogueAreaTrigger] Collider on '{gameObject.name}' is not set as trigger. Setting it now.");
            triggerCollider.isTrigger = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!ShouldTrigger(other)) return;
        
        if (triggerMode == TriggerMode.OnEnter || triggerMode == TriggerMode.Both)
        {
            TriggerDialogueEvent();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!ShouldTrigger(other)) return;
        
        if (triggerMode == TriggerMode.OnExit || triggerMode == TriggerMode.Both)
        {
            TriggerDialogueEvent();
        }
    }
    
    private bool ShouldTrigger(Collider other)
    {
        // Check if it's the player
        if (!other.CompareTag(playerTag))
            return false;
        
        // Check if already triggered (if triggerOnce is true)
        if (triggerOnce && hasTriggered)
            return false;
        
        return true;
    }
    
    private void TriggerDialogueEvent()
    {
        if (DialogueEventManager.Instance == null)
        {
            Debug.LogError($"[DialogueAreaTrigger] DialogueEventManager not found! Cannot trigger event '{eventName}'");
            return;
        }
        
        // Mark as triggered
        if (triggerOnce)
        {
            hasTriggered = true;
        }
        
        // Trigger with or without delay
        if (dialogueDelay > 0)
        {
            StartCoroutine(TriggerWithDelay());
        }
        else
        {
            TriggerEvent();
        }
    }
    
    private IEnumerator TriggerWithDelay()
    {
        yield return new WaitForSeconds(dialogueDelay);
        TriggerEvent();
    }
    
    private void TriggerEvent()
    {
        DialogueEventManager.Instance.TriggerEvent(eventType, eventName);
        Debug.Log($"[DialogueAreaTrigger] Triggered event: {eventType} ('{eventName}') from area '{gameObject.name}'");
    }
    
    /// <summary>
    /// Reset the trigger to allow it to activate again
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log($"[DialogueAreaTrigger] Reset trigger on '{gameObject.name}'");
    }
    
    /// <summary>
    /// Manually trigger the dialogue event
    /// </summary>
    public void ManualTrigger()
    {
        if (!hasTriggered || !triggerOnce)
        {
            TriggerDialogueEvent();
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Collider col = GetComponent<Collider>();
        if (col == null) return;
        
        Gizmos.color = gizmoColor;
        
        // Draw based on collider type
        if (col is BoxCollider boxCol)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(boxCol.center, boxCol.size);
            Gizmos.matrix = Matrix4x4.identity;
        }
        else if (col is SphereCollider sphereCol)
        {
            Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius * transform.lossyScale.x);
        }
        else if (col is CapsuleCollider capsuleCol)
        {
            // Simplified capsule visualization
            Gizmos.DrawSphere(transform.position + capsuleCol.center, capsuleCol.radius * transform.lossyScale.x);
        }
        
        // Draw label
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position, $"Dialogue: {eventName}");
        #endif
    }
}