using UnityEngine;

public class Observable : MonoBehaviour, IInteractable
{
    [SerializeField] private string clueName;
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 3f;

    private bool hasInteracted = false;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    public void Interact()
    {
        if (hasInteracted) return;

        hasInteracted = true;

        // Add clue
        if (!string.IsNullOrEmpty(clueName))
        {
            MainManager.mainManager.clueNames.Add(clueName);
        }

        // Show notification using MainManager
        if (notification != null)
        {
            notification.SetActive(true);
            MainManager.mainManager.HideAfterDelay(notification, notificationDuration);
        }

        // Disable collider so it is one-time
        if (col != null)
            col.enabled = false;
    }
}

