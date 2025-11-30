using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration for a character's dialogues based on events
/// </summary>
[CreateAssetMenu(fileName = "CharacterDialogue", menuName = "Dialogue/Character Dialogue Config")]
public class CharacterDialogueConfig : ScriptableObject
{
    [System.Serializable]
    public class EventDialogue
    {
        public DialogueEventType eventType;
        [Tooltip("For custom events, specify the event name")]
        public string customEventName;
        public Dialogue dialogue;
        [Tooltip("Should this dialogue only trigger once?")]
        public bool triggerOnce = false;
        [Tooltip("Delay before showing dialogue (seconds)")]
        public float delay = 0f;
        [Tooltip("Priority (higher = shown first if multiple match)")]
        public int priority = 0;
    }

    [Header("Character Info")]
    public string characterName;
    [TextArea(2, 4)]
    public string characterDescription;

    [Header("Event-Based Dialogues")]
    public List<EventDialogue> eventDialogues = new List<EventDialogue>();
}