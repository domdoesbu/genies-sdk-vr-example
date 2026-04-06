using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Node", menuName = "Node/Node Asset")]

public class DialogueNode : ScriptableObject
{
    public string dialogueText;
    public List<DialogueResponse> responses;
    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}
