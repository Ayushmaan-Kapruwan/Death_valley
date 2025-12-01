using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isLocked = true;
    public bool isOpen = false;

    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    private bool hasInteracted = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    [Header("Dialogue Settings")]
    [Tooltip("Dialogue event when the player tries opening a locked door")]
    [SerializeField] private string lockedDialogueEventName;

    [Tooltip("Dialogue event when the door successfully opens")]
    [SerializeField] private string openedDialogueEventName;

    [Tooltip("Delay before triggering dialogue")]
    [SerializeField] private float dialogueDelay = 0.3f;

    private void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0f, openAngle, 0f) * closedRotation;
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door unlocked!");
    }

    public void Interact()
    {
        if (hasInteracted) return;

        // ---------- Locked Door Dialogue ----------
        if (isLocked)
        {
            Debug.Log("Door is locked.");

            if (DialogueEventManager.Instance != null && !string.IsNullOrEmpty(lockedDialogueEventName))
            {
                MainManager.mainManager.TriggerDialogueWithDelay(lockedDialogueEventName, dialogueDelay);
            }

            return;
        }
        // -----------------------------------------

        // Door can be opened
        hasInteracted = true;
        isOpen = true;

        // ---------- Open Door Dialogue ----------
        if (DialogueEventManager.Instance != null && !string.IsNullOrEmpty(openedDialogueEventName))
        {
            MainManager.mainManager.TriggerDialogueWithDelay(openedDialogueEventName, dialogueDelay);
        }
        // ----------------------------------------

        StartCoroutine(OpenDoor());
    }

    private System.Collections.IEnumerator OpenDoor()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            transform.localRotation = Quaternion.Slerp(closedRotation, openRotation, t);
            yield return null;
        }
    }
}

