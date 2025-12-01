using UnityEngine;

public class DoorKey : MonoBehaviour, IInteractable
{
    [Header("Door To Unlock")]
    [SerializeField] private DoorInteractable doorToUnlock;

    [Header("Notification (Optional)")]
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 2f;

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

        // Destroy key
        Destroy(gameObject);
    }
}

