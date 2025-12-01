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
        if (isLocked)
        {
            Debug.Log("Door is locked.");
            return;
        }

        hasInteracted = true;
        isOpen = true;

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

