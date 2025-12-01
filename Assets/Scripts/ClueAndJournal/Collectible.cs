using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    [SerializeField] private string clueName;
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 3f;

    [Tooltip("Optional: Trigger a custom dialogue event with a different name")]
    [SerializeField] private string customDialogueEventName;

    [Tooltip("Delay before triggering dialogue (in seconds)")]
    [SerializeField] private float dialogueDelay = 0.5f;

    private bool collected = false;

    public void Interact()
    {
        if (collected) return;
        collected = true;

        // Add clue
        if (!string.IsNullOrEmpty(clueName))
        {
            MainManager.mainManager.clueNames.Add(clueName);
        }

        // Show notification
        if (notification != null)
        {
            notification.SetActive(true);
            MainManager.mainManager.HideAfterDelay(notification, notificationDuration);
        }

        // --- NEW: Trigger dialogue event through MainManager ---
        if (DialogueEventManager.Instance != null)
        {
            string eventName = string.IsNullOrEmpty(customDialogueEventName) ? clueName : customDialogueEventName;
            MainManager.mainManager.TriggerDialogueWithDelay(eventName, dialogueDelay);
        }
        // --------------------------------------------------------

        // Destroy after interaction
        Destroy(gameObject);
    }
}

