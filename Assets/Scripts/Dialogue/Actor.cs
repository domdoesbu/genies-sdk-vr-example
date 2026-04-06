using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    
    // Call this to trigger dialogue with an NPC. (like when clicking on the avatar)
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode, this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SpeakTo();
        }
    }
}
