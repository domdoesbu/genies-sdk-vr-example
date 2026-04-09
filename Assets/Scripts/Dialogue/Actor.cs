using System;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    public List<TargetItems> items = new List<TargetItems>();
    public bool spokenTo;

    [SerializeField] public Animator animator;


    // Call this to trigger dialogue with an NPC. (like when clicking on the avatar)
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode, this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !spokenTo)
        {
            SpeakTo();
            spokenTo = true;
            animator.SetBool("talking", true);
            animator.SetBool("walking", false);
            animator.SetBool("idle", false);

            foreach (TargetItems item in items) 
            {
                item.Validate();
            }
        }
    }

    // Animation



}
