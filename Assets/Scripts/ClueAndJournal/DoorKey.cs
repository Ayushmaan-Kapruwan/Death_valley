using UnityEngine;

public class DoorKey : MonoBehaviour, IInteractable
{
    [Header("Door To Unlock")]
    [SerializeField] private DoorInteractable doorToUnlock;

    [Header("Notification (Optional)")]
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 2f;

    [Header("Dialogue (Optional)")]
    [Tooltip("Optional: Trigger a custom dialogue event with a different name")]
    [SerializeField] private string customDialogueEventName;

    [Tooltip("Delay before triggering dialogue (in seconds)")]
    [SerializeField] private float dialogueDelay = 0.5f;

    public void Interact()
    {
        // Unlock door
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor();
            Debug.Log("Door unlocked: " + doorToUnlock.name);
        }

        // Notification
        if (notification != null)
        {
            notification.SetActive(true);
            MainManager.mainManager.HideAfterDelay(notification, notificationDuration);
        }

        // --- NEW: Trigger dialogue event ---
        if (DialogueEventManager.Instance != null)
        {
            string eventName = string.IsNullOrEmpty(customDialogueEventName)
                                ? gameObject.name // fallback name of the key object
                                : customDialogueEventName;

            MainManager.mainManager.TriggerDialogueWithDelay(eventName, dialogueDelay);
        }
        // ----------------------------------

        // Destroy key
        Destroy(gameObject);
    }
}

