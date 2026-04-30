using Genies.Sdk.Samples.Common;
using Oculus.Interaction;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//https://www.youtube.com/watch?v=pzaxC-P3sgs
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask, basketLayerMask, npcLayerMask, fridgeLayerMask;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField][Min(1)] private float hitRange = 3;
    private RaycastHit itemHit;
    private RaycastHit NPCHit;
    private RaycastHit basketHit;
    private RaycastHit fridgeHit;
    [SerializeField] private InputActionReference interactionInput, dropInput;
    public Transform avatarHand;
    [SerializeField] private GameObject inHandItem;
    public GameObject basket;
    [SerializeField] private GameObject NPC;
    public GameManager manager;
    public StarterAssetsInputs starterInput;
    public GeniesInputs geniesInputs;
    public Actor npcActor;
    public GameObject fridgeDoor;
    private void Start()
    {
        starterInput = FindAnyObjectByType<StarterAssetsInputs>();
        geniesInputs = FindAnyObjectByType<GeniesInputs>();
        manager = FindAnyObjectByType<GameManager>();
        interactionInput.action.performed += Interact;
        dropInput.action.performed += Drop;
    }
    private void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

        
        if(itemHit.collider != null)
        {
            itemHit.collider.GetComponent<Outline>()?.SetOutline(false);
            //pickUpUI.SetActive(false);
        }
        if (basketHit.collider != null)
        { 
            basketHit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
        }
        if (fridgeHit.collider != null)
        {
            fridgeHit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
        }
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out NPCHit, hitRange, npcLayerMask))
        {
            NPC = NPCHit.collider.gameObject;
            npcActor = NPC.GetComponent<Actor>();
            if (npcActor != null && !npcActor.spokenTo)
            {
                npcActor.dialogueManager.ShowInteractPrompt();
                npcActor.movement.Talking();
            }
            
        }
        else
        {
            if(npcActor != null)
            {
                npcActor.dialogueManager.HideInteractPrompt();
                if (!npcActor.spokenTo)
                {
                    npcActor.movement.Walking();
                }
            }
            
        }
        // If item in hand, don't detect anything else
        if (inHandItem != null )
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out basketHit, hitRange, basketLayerMask))
            {
                basketHit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            }
            return;
        }

        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out itemHit, hitRange, pickableLayerMask))
        {
            itemHit.collider.GetComponent<Outline>()?.SetOutline(true);
            //pickUpUI.SetActive(true);
        }
        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out fridgeHit, hitRange, fridgeLayerMask))
        {
            fridgeHit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
        }
    }

    public void FindAvatarHand()
    {
        avatarHand = GameObject.Find("RightHandBind").transform;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Rigidbody rb;
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
            Debug.Log("E :: NPC !null " + NPCHit.collider.name);
            manager.DisableMove();
           
            Actor actor = NPC.GetComponent<Actor>();
            if (actor != null)
            {
                actor.StartDialogue();
            }
            else
                Debug.Log("Actor is null");
            
        }
       
        else if (itemHit.collider != null && inHandItem == null)
        {
            rb = itemHit.collider.GetComponent<Rigidbody>();
            Debug.Log("E :: NPC null, hit !null, inhanditem null, basket component null: " + itemHit.collider.name);
            inHandItem = itemHit.collider.gameObject;
            inHandItem.transform.SetParent(avatarHand.transform, false);
            inHandItem.transform.localPosition = Vector3.zero;
            inHandItem.transform.rotation = Quaternion.identity;

            if (rb != null)
            {
                rb.isKinematic = true;
            }
            return;
        }
        else if (basketHit.collider != null && inHandItem != null && basketHit.collider.GetComponent<Basket>() != null)
        {
            rb = basketHit.collider.GetComponent<Rigidbody>();
            Debug.Log("E :: NPC null, hit !null, inhanditem !null, basket component !null: " + basketHit.collider.name);
            inHandItem.transform.SetParent(basketHit.collider.transform, false);
            inHandItem.transform.localPosition = Vector3.zero;
            inHandItem.transform.rotation = Quaternion.identity;
            if (rb != null)
            {
                rb.isKinematic = true;
            }
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
}
