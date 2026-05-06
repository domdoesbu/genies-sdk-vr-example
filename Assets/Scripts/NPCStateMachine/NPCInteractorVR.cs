using UnityEngine;

public class NPCInteractorVR : MonoBehaviour
{
    private Actor actor;

    private void Awake()
    {
        actor = GetComponent<Actor>();
    }
    public void OnHoverStart()
    {
        if(actor != null && !actor.spokenTo)
        {
            actor.dialogueManager.ShowInteractPrompt();
            if(actor.movement != null)
            {
                actor.movement.Talking();
            }
        }
    }

    public void OnHoverEnd()
    {
        if(actor != null && !actor.spokenTo)
        {
            actor.dialogueManager.HideInteractPrompt();
            if (actor.movement != null)
            {
                actor.movement.Walking();
            }        
        }
    }

    public void OnSelect()
    {
        if (actor != null && !actor.spokenTo)
        {
            actor.StartDialogue();
        }
    }
}
