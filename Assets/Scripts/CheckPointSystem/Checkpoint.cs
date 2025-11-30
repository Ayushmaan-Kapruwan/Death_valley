using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Player tag to detect")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Visual feedback when checkpoint is activated (optional)")]
    [SerializeField] private GameObject activatedEffect;

    [Tooltip("Only trigger once?")]
    [SerializeField] private bool oneTimeUse = true;

    private bool hasBeenActivated = false;

    private void Start()
    {
        // Ensure the collider is set to trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        if (activatedEffect != null)
        {
            activatedEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if already activated and one-time use
        if (oneTimeUse && hasBeenActivated)
            return;

        // Check if it's the player
        if (other.CompareTag(playerTag))
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(transform);
            hasBeenActivated = true;

            // Show visual feedback
            if (activatedEffect != null)
            {
                activatedEffect.SetActive(true);
            }

            Debug.Log($"Checkpoint '{gameObject.name}' activated!");
        }
        else
        {
            Debug.LogError("CheckpointManager not found in scene!");
        }
    }

    // Public method to manually reset checkpoint (if needed)
    public void ResetCheckpoint()
    {
        hasBeenActivated = false;
        if (activatedEffect != null)
        {
            activatedEffect.SetActive(false);
        }
    }
}