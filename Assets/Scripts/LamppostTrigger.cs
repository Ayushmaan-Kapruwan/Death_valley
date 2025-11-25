using UnityEngine;

public class LamppostTrigger : MonoBehaviour
{
    [Tooltip("Optional: name of the Player tag. Default: Player")]
    [SerializeField] private string playerTag = "Player";

    private FlashlightController flashlight;
    private bool wasFlashlightOnBeforeEntering = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            flashlight = player.GetComponentInChildren<FlashlightController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeInHierarchy) return;
        if (other.CompareTag(playerTag))
        {
            if (SanityManager.Instance != null)
                SanityManager.Instance.SetUnderLamp(true);

            // Remember if flashlight was on before forcing it off
            if (flashlight != null)
            {
                wasFlashlightOnBeforeEntering = flashlight.IsOn;
                flashlight.TurnOff();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!gameObject.activeInHierarchy) return;
        if (other.CompareTag(playerTag))
        {
            // Update lamp state FIRST
            if (SanityManager.Instance != null)
                SanityManager.Instance.SetUnderLamp(false);

            // Only turn flashlight back on if it was on before entering
            if (flashlight != null && wasFlashlightOnBeforeEntering)
            {
                flashlight.TurnOn();
            }
        }
    }
}
