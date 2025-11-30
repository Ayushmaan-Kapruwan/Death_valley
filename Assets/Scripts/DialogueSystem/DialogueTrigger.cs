using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        DialogueManager manager = FindFirstObjectByType<DialogueManager>();
        manager.StartDialogue(dialogue);
    }
}
