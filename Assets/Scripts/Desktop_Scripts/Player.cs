using Genies.Sdk.Samples.Common;
using Oculus.Interaction;
using StarterAssets;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//https://www.youtube.com/watch?v=pzaxC-P3sgs
public class Player : MonoBehaviour
{
    [Header("Ray casts")]
    [SerializeField] private LayerMask pickableLayerMask, basketLayerMask, npcLayerMask, fridgeLayerMask;
    private RaycastHit itemHit;
    private RaycastHit NPCHit;
    private RaycastHit basketHit;
    private RaycastHit fridgeHit;

    [Header("Player")]
    [SerializeField] private Transform playerCameraTransform;
    public Transform avatarHand;
    public GameManager manager;

    [Header("Inputs")]
    public StarterAssetsInputs starterInput;
    public GeniesInputs geniesInputs;
    [SerializeField] private InputActionReference interactionInput, dropInput, pauseInput;
    
    [Header("Game Objects")]
    public GameObject fridgeDoor;
    public GameObject basket;
    [SerializeField] private GameObject inHandItem;
    [SerializeField] private GameObject NPC;

    [Header("UI")]
    public TextMeshProUGUI interactText;
    public GameObject interactUI;
    public GameObject pauseUI;
    public Vector2 pauseUISize;
    public bool paused = false;
    [SerializeField] private GameObject pickUpUI;

    [Header("Other")]
    [SerializeField][Min(1)] private float hitRange = 3;
    public Actor npcActor;

    private void Start()
    {
        starterInput = FindAnyObjectByType<StarterAssetsInputs>();
        geniesInputs = FindAnyObjectByType<GeniesInputs>();
        manager = FindAnyObjectByType<GameManager>();
        interactionInput.action.performed += Interact;
        dropInput.action.performed += Drop;
        pauseInput.action.performed += Pause;
        pauseUISize = pauseUI.transform.localScale;
        pauseUI.SetActive(false);
    }
    private void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

        // Reset highlights on all objects
        if(itemHit.collider != null)
        {
            itemHit.collider.GetComponent<Outline>()?.SetOutline(false);
        }
        if (basketHit.collider != null)
        { 
            basketHit.collider.GetComponent<Outline>()?.SetOutline(false);
        }
        if (fridgeHit.collider != null)
        {
            fridgeHit.collider.GetComponent<Outline>()?.SetOutline(false);
        }

        // If interacting with NPC
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out NPCHit, hitRange, npcLayerMask))
        {
            NPC = NPCHit.collider.gameObject;
            npcActor = NPC.GetComponent<Actor>();
            
            if (npcActor != null && !npcActor.spokenTo)
            {
                npcActor.dialogueManager.ShowInteractPrompt();
                if(npcActor.movement != null)
                {
                    npcActor.movement.Talking();
                }
                    
            }
            return;
        }
        else
        {
            if (npcActor != null)
            {
                npcActor.dialogueManager.HideInteractPrompt();
                if (!npcActor.spokenTo)
                {
                    if(npcActor.movement != null)
                        npcActor.movement.Walking();
                }
            }
            
        }
        // If item in hand, don't detect anything else
       
        // If item in hand and hover basket
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out basketHit, hitRange, basketLayerMask))
        {
            interactUI.SetActive(true);
            interactText.text = "Press E to pick up basket";
            basketHit.collider.GetComponent<Outline>()?.SetOutline(true);
            return;
        }
        
        
        // If fridge interaction
        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out fridgeHit, hitRange, fridgeLayerMask))
        {
            interactUI.SetActive(true);
            interactText.text = "Press E to interact";
            fridgeHit.collider.GetComponent<Outline>()?.SetOutline(true);
            return;
        }
        // If hovering a pickable item
        else if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out itemHit, hitRange, pickableLayerMask))
        {
            interactUI.SetActive(true);
            interactText.text = "Press E to pick up item";
            itemHit.collider.GetComponent<Outline>()?.SetOutline(true);
            return;
        }
        else
        {
            interactText.text = "Press E to interact";
            interactUI.SetActive(false);
        }
    }

    public void FindAvatarHand()
    {
        avatarHand = GameObject.Find("RightHandBind").transform;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Rigidbody rb;

        // Pick up basket
        if(basketHit.collider != null)
        {
            rb = basketHit.collider.GetComponent<Rigidbody>();
            
            basketHit.collider.gameObject.transform.SetParent(avatarHand.transform, false);
            basketHit.collider.gameObject.transform.localPosition = new Vector3(0.133f, 0.339f, 0.053f);
            basketHit.collider.gameObject.transform.localScale = new Vector3(0.71f, 0.71f, 0.71f);
            basketHit.collider.gameObject.transform.rotation = Quaternion.identity;

            if (rb != null)
            {
                rb.isKinematic = true;
            }
            return;

        }
        if (fridgeHit.collider != null)
        {
            fridgeDoor = fridgeHit.collider.gameObject;
            FridgeDoor doorScript = fridgeDoor.GetComponent<FridgeDoor>();
            
            if (doorScript != null)
            {
                doorScript.isClosed = !doorScript.isClosed;
                doorScript.interact = true;
            }
        }
        else if (NPC != null && NPCHit.collider != null && NPCHit.collider.GetComponent<Actor>() != null)
        {
            manager.DisableMove();
            Actor actor = NPC.GetComponent<Actor>();

            if (actor != null)
            {
                actor.StartDialogue();
            }
        }
        
        else if (itemHit.collider != null && inHandItem == null)
        {
            rb = itemHit.collider.GetComponent<Rigidbody>();
            inHandItem = itemHit.collider.gameObject;
            inHandItem.transform.SetParent(avatarHand.transform, false);
            inHandItem.transform.localPosition = Vector3.zero;
            inHandItem.transform.rotation = Quaternion.identity;
            return;
        }
    }

    public void Drop(InputAction.CallbackContext obj)
    {
        Rigidbody rb;
        if(inHandItem != null)
        {
            rb = inHandItem.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.isKinematic = false;
            }
            inHandItem.transform.SetParent(null);
            inHandItem = null;
        }
    }

    public void Pause(InputAction.CallbackContext obj)
    {
        paused = !paused;
        pauseUI.SetActive(paused);
    }

}
