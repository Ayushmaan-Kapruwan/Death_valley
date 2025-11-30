using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    [Header("Clue (Optional)")]
    [SerializeField] private string clueName;
    [SerializeField] private bool addClueToManager = true;

    [Header("Dialogue (Optional)")]
    [SerializeField] private DialogueTrigger dialogueTrigger;

    [Header("Notification (Optional)")]
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 2f;

    public void Interact()
    {
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

        // 4. Destroy the object immediately
        Destroy(gameObject);
    }
}

