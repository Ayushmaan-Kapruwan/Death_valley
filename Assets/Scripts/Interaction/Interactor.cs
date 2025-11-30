using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractorRange = 3f;
    public GameObject interactCanvas;

    private IInteractable currentTarget;

    private void Update()
    {
        DetectInteractable();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact();
            interactCanvas.SetActive(false);
        }
    }

    private void DetectInteractable()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractorRange))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactObj))
            {
                currentTarget = interactObj;
                interactCanvas.SetActive(true);
                return;
            }
        }

        currentTarget = null;
        interactCanvas.SetActive(false);
    }
}

