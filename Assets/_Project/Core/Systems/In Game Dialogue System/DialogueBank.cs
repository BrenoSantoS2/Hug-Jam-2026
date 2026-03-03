using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueBank", menuName = "Dialogue/Dialogue Bank")]
public class DialogueBank : ScriptableObject
{
    [System.Serializable]
    public class DialogueEntry
    {
        public DialogueType type;
        [TextArea(2, 5)]
        public List<string> dialogues = new List<string>();
    }

    public List<DialogueEntry> entries = new List<DialogueEntry>();

    public string GetRandomDialogue(DialogueType type)
    {
        DialogueEntry entry = entries.Find(e => e.type == type);
        
        if (entry == null || entry.dialogues.Count == 0)
            return null;

        int randomIndex = Random.Range(0, entry.dialogues.Count);
        return entry.dialogues[randomIndex];
    }
}
