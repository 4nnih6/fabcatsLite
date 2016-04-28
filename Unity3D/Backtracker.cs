using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This script adds the ability to backtrack conversations. To backtrack, call Backtrack(true).
    /// The bool parameter specifies whether to backtrack to an NPC line, which is what you usually
    /// want to do; otherwise you'll keep backtracking to the same response menu instead of going
    /// back to a previous NPC line.
    /// </summary>
    public class Backtracker : MonoBehaviour
    {

        public bool debug;

        public struct IDRecord
        {
            public int conversationID;
            public int entryID;
            public bool isNPC;

            public IDRecord(int conversationID, int entryID, bool isNPC)
            {
                this.conversationID = conversationID;
                this.entryID = entryID;
                this.isNPC = isNPC;
            }
        }

        private Stack<IDRecord> stack = new Stack<IDRecord>();
        private Transform conversationActor;
        private Transform conversationConversant;
        private bool isBacktracking = false;

        public void OnConversationStart(Transform actor)
        {
            if (!isBacktracking)
            {
                // If we're really starting a new conversation (not just backtracking), initialize:
                stack.Clear();
                conversationActor = DialogueManager.CurrentActor;
                conversationConversant = DialogueManager.CurrentConversant;
                if (debug) Debug.Log("Backtracker: Starting a new conversation. Clearing stack.");
            }
        }

        public void OnConversationLine(Subtitle subtitle)
        {
            // Record the current entry:
            var isNPC = subtitle.speakerInfo.characterType == CharacterType.NPC;
            stack.Push(new IDRecord(subtitle.dialogueEntry.conversationID, subtitle.dialogueEntry.id, isNPC));
            if (debug) Debug.Log("Backtracker: Recording dialogue entry " + subtitle.dialogueEntry.conversationID + ":" + subtitle.dialogueEntry.id + " on stack: '" + subtitle.formattedText.text + "' (" + subtitle.speakerInfo.characterType + ").");
        }

        // Call this method to go back:
        public void Backtrack(bool toPreviousNPCLine)
        {
            if (stack.Count < 2) return;
            stack.Pop(); // Pop current entry.
            var destination = stack.Pop(); // Pop previous entry.
            if (toPreviousNPCLine)
            {
                while (!destination.isNPC && stack.Count > 0)
                {
                    destination = stack.Pop(); // Keep popping until we get an NPC line.
                }
                if (!destination.isNPC) return;
            }
            var title = DialogueManager.MasterDatabase.GetConversation(destination.conversationID).Title;
            DialogueManager.StopConversation();
            isBacktracking = true;
            if (debug) Debug.Log("Backtracker: Backtracking to '" + title + "' entry " + destination.entryID);
            DialogueManager.StartConversation(title, conversationActor, conversationConversant, destination.entryID);
            isBacktracking = false;
        }
    }
}
