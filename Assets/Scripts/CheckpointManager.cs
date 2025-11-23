using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private Transform initialSpawnPoint;
    
    private Transform currentCheckpoint;
    private CharacterController playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterController>();

        if (initialSpawnPoint != null)
        {
            currentCheckpoint = initialSpawnPoint;
        }
    }

    // Called by Checkpoint triggers when player passes through them
    public void SetCheckpoint(Transform checkpointTransform)
    {
        currentCheckpoint = checkpointTransform;
        Debug.Log($"Checkpoint updated: {checkpointTransform.name}");
    }

    // Respawns the player at the last checkpoint
    public void RespawnPlayer()
    {
        if (playerController == null || currentCheckpoint == null)
        {
            Debug.LogError("CheckpointManager: Cannot respawn - missing player or checkpoint!");
            return;
        }

        // Disable CharacterController to allow position change
        playerController.enabled = false;
        
        // Move player to checkpoint position
        playerController.transform.position = currentCheckpoint.position;
        playerController.transform.rotation = currentCheckpoint.rotation;
        
        // Re-enable CharacterController
        playerController.enabled = true;

        Debug.Log($"Player respawned at: {currentCheckpoint.name}");
    }

    /// <summary>
    /// Gets the current checkpoint position (useful for debugging)
    /// </summary>
    public Vector3 GetCurrentCheckpointPosition()
    {
        return currentCheckpoint != null ? currentCheckpoint.position : Vector3.zero;
    }
}