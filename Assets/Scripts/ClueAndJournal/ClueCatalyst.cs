using System.Collections;
using UnityEngine;

public class ClueCatalyst : MonoBehaviour, IInteractable
{
    [SerializeField] private string clue;
    [SerializeField] private GameObject notification;
    [SerializeField] private float notificationDuration = 3f;
    [Tooltip("Optional: Trigger a custom dialogue event with a different name")]
    [SerializeField] private string customDialogueEventName;
    [Tooltip("Delay before triggering dialogue (in seconds)")]
    [SerializeField] private float dialogueDelay = 0.5f;
    
    private bool clueAdded = false;
    private Coroutine hideNotificationCoroutine;

    // Implement IInteractable interface
    public void Interact()
    {
        CreateClue();
    }

    public void CreateClue()
    {
        Debug.Log($"CreateClue called. Clue: '{clue}', ClueAdded: {clueAdded}");

        if (clue != null && !clueAdded)
        {
            clueAdded = true;
            MainManager.mainManager.clueNames.Add(clue);
            Debug.Log($"Clue added to MainManager. Total clues: {MainManager.mainManager.clueNames.Count}");

            // Print all clues
            foreach (string c in MainManager.mainManager.clueNames)
            {
                Debug.Log($"  - {c}");
            }

            // Trigger dialogue event when clue is found
            if (DialogueEventManager.Instance != null)
            {
                // Use custom event name if specified, otherwise use the clue name
                string eventName = string.IsNullOrEmpty(customDialogueEventName) ? clue : customDialogueEventName;
                
                if (dialogueDelay > 0)
                {
                    StartCoroutine(TriggerDialogueWithDelay(eventName));
                }
                else
                {
                    DialogueEventManager.Instance.TriggerEvent(DialogueEventType.OnClueFound, eventName);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Clue NOT added. Clue is null: {clue == null}, Already added: {clueAdded}");
        }

        if (notification != null)
        {
            notification.SetActive(true);

            if (hideNotificationCoroutine != null)
            {
                StopCoroutine(hideNotificationCoroutine);
            }

            hideNotificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
        }
    }

    private IEnumerator TriggerDialogueWithDelay(string eventName)
    {
        yield return new WaitForSeconds(dialogueDelay);
        
        if (DialogueEventManager.Instance != null)
        {
            DialogueEventManager.Instance.TriggerEvent(DialogueEventType.OnClueFound, eventName);
        }
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);

        if (notification != null)
        {
            notification.SetActive(false);
        }
    }
}

