using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    [SerializeField] private string clueName;
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 3f;

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

        // Destroy after interaction
        Destroy(gameObject);
    }
}

