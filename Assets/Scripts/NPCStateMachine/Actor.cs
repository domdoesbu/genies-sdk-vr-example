using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Actor : MonoBehaviour
{
    // NAV
    private float speed;
    public NavMeshAgent agent;
    public Animator animator;
    public AvatarMovement movement;
    public DialogueManager dialogueManager;
    public string Name;
    public Dialogue GN_dialogue;
    public Dialogue G_dialogue;
    public List<TargetItems> items = new List<TargetItems>();
    public bool spokenTo;
    public BoxCollider boxCollider;
    public GameManager gameManager;

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
        if(gameManager.genderNeutral)
            dialogueManager.StartDialogue(Name, GN_dialogue.RootNode);
        else
            dialogueManager.StartDialogue(Name, G_dialogue.RootNode);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !spokenTo)
        {
            boxCollider.enabled = false;
            movement.Talking();
            SpeakTo();
            spokenTo = true;
            foreach (TargetItems item in items) 
            {
                item.Validate();
            }
        }
    }
}
