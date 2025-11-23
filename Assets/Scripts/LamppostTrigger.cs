using UnityEngine;

public class LamppostTrigger : MonoBehaviour
{
    [Tooltip("Optional: name of the Player tag. Default: Player")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeInHierarchy) return;
        if (other.CompareTag(playerTag))
        {
            if (SanityManager.Instance != null)
                SanityManager.Instance.SetUnderLamp(true);

            // Force the flashlight (torch) off as soon as the player enters
            SanityManager.Instance.ForceFlashlightOff();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!gameObject.activeInHierarchy) return;
        if (other.CompareTag(playerTag))
        {
            if (SanityManager.Instance != null)
                SanityManager.Instance.SetUnderLamp(false);
        }
    }
}
