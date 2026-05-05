using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Actor : MonoBehaviour
{
    public List<TargetItems> items = new List<TargetItems>();
    public GameManager gameManager;

    // NAV
    private float speed;
    public NavMeshAgent agent;
    public Animator animator;
    public AvatarMovement movement;

    // UI
    public string Name;
    public BoxCollider boxCollider;
    
    // Dialogue
    public bool spokenTo;
    public Dialogue GN_dialogue;
    public Dialogue G_dialogue;
    public DialogueManager dialogueManager;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<AvatarMovement>();
        dialogueManager = GetComponent<DialogueManager>();
        boxCollider = GetComponent<BoxCollider>();
        gameManager = FindAnyObjectByType<GameManager>();
    }


    public float CurrentSpeed
    {
        get { return agent.velocity.magnitude; }
    }

    // Call this to trigger dialogue with an NPC. (like when clicking on the avatar)
    public void SpeakTo()
    {
        if(gameManager.genderNeutral && dialogueManager != null)
            dialogueManager.StartDialogue(GN_dialogue.RootNode);
        else if (dialogueManager != null)
            dialogueManager.StartDialogue(G_dialogue.RootNode);    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" && !spokenTo && dialogueManager != null)
    //    {
    //        dialogueManager.ShowInteractPrompt();
    //        if(movement != null)
    //            movement.Talking();
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (gameManager.VR && other.gameObject.tag == "Player" && !spokenTo && dialogueManager != null && OVRInput.GetDown(OVRInput.RawButton.Y))
    //    {
    //        StartDialogue();
    //    }
    //}

    public void StartDialogue()
    {
        dialogueManager.HideInteractPrompt();
        boxCollider.enabled = false;
        if(movement != null)
            movement.Talking();
        SpeakTo();
        spokenTo = true;
        foreach (TargetItems item in items)
        {
            item.Validate();
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" && !spokenTo)
    //    {
    //        if(dialogueManager != null)
    //            dialogueManager.HideInteractPrompt();
    //        if(movement != null)
    //            movement.Walking();
    //    }
    //}
}
