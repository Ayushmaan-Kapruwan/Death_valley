using System.Collections;
using UnityEngine;

public class ClueCatalyst : MonoBehaviour
{
    [SerializeField] private string clue;
    [SerializeField] private GameObject notification;
    [SerializeField] private KeyCode removeClue = KeyCode.R;
    [SerializeField] private float notificationDuration = 3f;
    private bool clueAdded = false;
    private Coroutine hideNotificationCoroutine;

    private void Update()
    {
        
        if (Input.GetKeyDown(removeClue))
        {
            CompleteClue();
        }
    }

    public void CreateClue()
    {
        Debug.Log($"CreateClue called. Clue: '{clue}', ClueAdded: {clueAdded}");
        
        if (clue != null && !clueAdded)
        {
            clueAdded = !clueAdded;
            MainManager.mainManager.clueNames.Add(clue);
            Debug.Log($"Clue added to MainManager. Total clues: {MainManager.mainManager.clueNames.Count}");
            
            // Print all clues
            foreach (string c in MainManager.mainManager.clueNames)
            {
                Debug.Log($"  - {c}");
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

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        
        if (notification != null)
        {
            notification.SetActive(false);
        }
    }
    
    public void CompleteClue()
    {
        if (clue != null && MainManager.mainManager.clueNames.Contains(clue))
        {
            MainManager.mainManager.clueNames.Remove(clue);
            Debug.Log($"Clue removed: {clue}");
        }
    }
}
