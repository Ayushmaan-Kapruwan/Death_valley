using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{
    public List<string> clueNames = new();
    public static MainManager mainManager;

    private void Awake()
    {
        if (mainManager != null)
        {
            Destroy(gameObject);
            return;
        }

        mainManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void HideAfterDelay(GameObject obj, float delay)
    {
        StartCoroutine(HideRoutine(obj, delay));
    }

    private IEnumerator HideRoutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
            obj.SetActive(false);
    }

    // DIALOGUE DELAY HANDLER (new)
    public void TriggerDialogueWithDelay(string eventName, float delay)
    {
        StartCoroutine(DialogueRoutine(eventName, delay));
    }

    private IEnumerator DialogueRoutine(string eventName, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (DialogueEventManager.Instance != null)
        {
            DialogueEventManager.Instance.TriggerEvent(DialogueEventType.OnClueFound, eventName);
        }
    }
}

