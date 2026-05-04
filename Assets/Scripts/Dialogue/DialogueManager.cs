using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // UI elements
    public GameObject dialogueParent; // Main container
    public TextMeshProUGUI dialogueName, dialogueText; // Name and main text
    public GameObject responseButtonPrefab; // Prefab for generating response buttons
    public Transform responseButtonContainer; // container holding the response buttons
    public AvatarMovement movement;
    public DialogueVariables dialogueVariables;
    public Actor actor;
    // Interact prompt
    public GameObject interactPrompt;
    public GameManager gameManager;
    public bool meetUpNPC;
    
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        dialogueVariables = FindAnyObjectByType<DialogueVariables>();
        actor = GetComponent<Actor>();
        HideDialogue();
        HideInteractPrompt();
    }

    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(DialogueNode node)
    {
        // Display the dialogue UI
        ShowDialogue();
        if (meetUpNPC && actor != null)
        {
            actor.movement.MeetUp();
        }
        // Set dialogue title and body text
        dialogueName.text = node.npcName;
        dialogueText.text = DialogueParser.Parse(node.dialogueText, dialogueVariables, gameManager.pluralVerbage);
        
        // Remove any existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create and setup response buttons based on current dialogue node
        foreach (DialogueResponse response in node.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
        }
    }

    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response)
    {
        // Check if there's a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(response.nextNode); // Start next dialogue
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
            if(movement != null)
                movement.RandomizeState();
            if (!gameManager.VR)
            {
                gameManager.EnableMove();
                actor.movement.Walking();
            }
        }
    }

    // Hide the dialogue UI
    public void HideDialogue()
    {
        dialogueParent.SetActive(false);
    }

    // Show the dialogue UI
    private void ShowDialogue()
    {
        dialogueParent.SetActive(true);
    }

    // Check if dialogue is currently active
    public bool IsDialogueActive()
    {
        return dialogueParent.activeSelf;
    }

    public void HideInteractPrompt()
    {
        interactPrompt.SetActive(false);
    }

    public void ShowInteractPrompt()
    {
        interactPrompt.SetActive(true);
    }

}
