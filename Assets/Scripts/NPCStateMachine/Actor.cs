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

    public string Name;
    public Dialogue Dialogue;
    public List<TargetItems> items = new List<TargetItems>();
    public bool spokenTo;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public float CurrentSpeed
    {
        get { return agent.velocity.magnitude; }
    }


    // Call this to trigger dialogue with an NPC. (like when clicking on the avatar)
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode, this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !spokenTo)
        {
            movement.Talking();
            SpeakTo();
            spokenTo = true;
            foreach (TargetItems item in items) 
            {
                item.Validate();
            }
        }
    }

    // Animation



}
