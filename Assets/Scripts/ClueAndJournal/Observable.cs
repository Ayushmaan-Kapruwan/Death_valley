using UnityEngine;

public class Observable : MonoBehaviour, IInteractable
{
    [Header("Clue (Optional)")]
    [SerializeField] private string clueName;
    [SerializeField] private bool addClueToManager = true;

    [Header("Dialogue (Optional)")]
    [SerializeField] private DialogueTrigger dialogueTrigger;

    [Header("Notification (Optional)")]
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 2f;

    private bool hasBeenObserved = false;

    public void Interact()
    {
        if (hasBeenObserved) return; // Already observed, do nothing

        hasBeenObserved = true;

        // 1. Add Clue
        if (addClueToManager && !string.IsNullOrEmpty(clueName))
        {
            MainManager.mainManager.clueNames.Add(clueName);
            Debug.Log("Clue added: " + clueName);
        }

        // 2. Dialogue
        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();
        }

        // 3. Notification
        if (notification != null)
        {
            notification.SetActive(true);
            MainManager.mainManager.HideAfterDelay(notification, notificationDuration);
        }

        // 4. Disable interaction so "Press E" doesn't appear
        // This effectively tells Interactor there's nothing to interact with
        // by disabling the collider (optional: or just remove IInteractable reference)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}

