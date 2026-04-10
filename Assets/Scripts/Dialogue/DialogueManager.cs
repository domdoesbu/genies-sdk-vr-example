using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // UI elements
    public GameObject dialogueParent; // Main container
    public TextMeshProUGUI dialogueName, dialogueText; // Name and main text
    public GameObject responseButtonPrefab; // Prefab for generating response buttons
    public Transform responseButtonContainer; // container holding the response buttons
    public AvatarMovement movement;

    public GameObject NPC;
    private void Awake()
    {
        // Makes sure we only have one dia-manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // on awake, make sure it is hidden
        HideDialogue();
    }

    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(string title, DialogueNode node, GameObject NPC)
    {
        this.NPC = NPC;
        //dialogueParent.transform.position = new Vector3(NPC.transform.position.x + 1.0f, NPC.transform.position.y, NPC.transform.rotation.y);
        // Display the dialogue UI
        ShowDialogue();

        // Set dialogue title and body text
        dialogueName.text = title;
        dialogueText.text = node.dialogueText;

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
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title));
        }
    }

    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response, string title)
    {
        // Check if there's a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode, NPC); // Start next dialogue
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
            movement.RandomizeState();
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
}
