using UnityEngine;

public class Sphere : MonoBehaviour, IInteractable {
    private ClueCatalyst clueCatalyst;

    private void Awake()
    {
        clueCatalyst = GetComponent<ClueCatalyst>();
        
        if (clueCatalyst == null)
        {
            Debug.LogError($"[SPHERE] ClueCatalyst component NOT FOUND on {gameObject.name}! Add it in Inspector.", this);
        }
        else
        {
            Debug.Log($"[SPHERE] ClueCatalyst found on {gameObject.name}");
        }
    }

    public void Interact()
    {
        Debug.Log($"[SPHERE] ===== INTERACT CALLED ON {gameObject.name} =====", this);
        Debug.Log($"[SPHERE] Random number: {Random.Range(0, 100)}");
        
        // Add the clue when interacted with
        if (clueCatalyst != null)
        {
            Debug.Log("[SPHERE] ClueCatalyst exists, calling CreateClue()...");
            clueCatalyst.CreateClue();
        }
        else
        {
            Debug.LogError("[SPHERE] ClueCatalyst is NULL - cannot add clue!", this);
        }
    }
}
