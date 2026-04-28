using Genies.Sdk.Samples.Common;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//https://www.youtube.com/watch?v=pzaxC-P3sgs
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask, basketLayerMask, npcLayerMask;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField][Min(1)] private float hitRange = 3;
    private RaycastHit hit;
    [SerializeField] private InputActionReference interactionInput, dropInput;
    public Transform avatarHand;
    [SerializeField] private GameObject inHandItem;
    public GameObject basket;
    [SerializeField] private GameObject NPC;
    public GameManager manager;
    public StarterAssetsInputs starterInput;
    public GeniesInputs geniesInputs;
    private void Start()
    {
        starterInput = FindAnyObjectByType<StarterAssetsInputs>();
        geniesInputs = FindAnyObjectByType<GeniesInputs>();
        manager = FindAnyObjectByType<GameManager>();
        interactionInput.action.performed += Interact;
    }
    private void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

        
        if(hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            //pickUpUI.SetActive(false);
        }
        
        // If item in hand, don't detect anything else
        if (inHandItem != null )
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, basketLayerMask))
            {
                hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            }
            return;
        }

        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            //pickUpUI.SetActive(true);
        }

        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, npcLayerMask))
        {
            NPC = hit.collider.gameObject;
        }
    }

    public void FindAvatarHand()
    {
        avatarHand = GameObject.Find("RightHandBind").transform;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (NPC == null && hit.collider != null && inHandItem == null && hit.collider.GetComponent<Basket>() == null)
        {

            Debug.Log(hit.collider.name);
            inHandItem = hit.collider.gameObject;
            inHandItem.transform.SetParent(avatarHand.transform, false);
            inHandItem.transform.localPosition = Vector3.zero;
            inHandItem.transform.rotation = Quaternion.identity;

            if (rb != null)
            {
                rb.isKinematic = true;
            }
            return;
        }
        else if (NPC == null && hit.collider != null && inHandItem != null && hit.collider.GetComponent<Basket>() != null)
        {
            
            inHandItem.transform.SetParent(hit.collider.transform, false);
            inHandItem.transform.localPosition = Vector3.zero;
            inHandItem.transform.rotation = Quaternion.identity;
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }

        if(NPC != null)
        {
            manager.DisableMove();
           
            NPC.GetComponent<Actor>().StartDialogue();
            NPC = null;
        }
    }
}
