using Genies.Sdk.Samples.Common;
using Meta.XR.Movement;
using StarterAssets;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("VR?")]
    public bool VR;

    [Header("Grocery List")]
    [SerializeField] public int[] groceryItemCount = new int[5];
    [SerializeField] public TextMeshProUGUI groceryList;
    private string[] groceryItemString = new string[5];

    [Header("Hint list")]
    [SerializeField] public TextMeshProUGUI[] itemHints = new TextMeshProUGUI[5];
    private string[] itemHintsText = new string[5];

    [Header("Inputs")]
    public GeniesInputs geniesInputs;
    public StarterAssetsInputs starterInput;

    [Header("Health")]
    public Health health;

    [Header("Toggles")]
    public bool genderNeutral = false;
    public bool pluralVerbage = false;
    public bool disableMove = true;
    private void Start()
    {
        itemHintsText[0] = "Sweet treat, hole in the middle, white icing, sharable.";
        itemHintsText[1] = "Fruit, three variations, the green one.";
        itemHintsText[2] = "In the fridge, comes in packs of 3.";
        itemHintsText[3] = "Bath toy.";
        itemHintsText[4] = "Fruit, bottom shelf, no leafs.";


        groceryItemString[0] = " &*(^%#";
        groceryItemString[1] = " !#()&@$";
        groceryItemString[2] = " ~><:@#(*& ";
        groceryItemString[3] = " >:#**@&$";
        groceryItemString[4] = " >{}@!#*$&!";

        groceryList.text = "";
        for (int i = 0; i < groceryItemCount.Length; i++)
        {
            groceryList.text += groceryItemCount[i] + "x" + groceryItemString[i] + "\n";
        }
    }

    public void DecreaseHealth()
    {
        health.DecreaseHealth();
        if(health.healthCount == 0)
        {
            GameLost();
        }
    }

    public void UpdateList()
    {
        groceryList.text = "";
        for (int i = 0; i < groceryItemCount.Length; i++)
        {
            if (groceryItemCount[i] == 0)
            {
                groceryItemString[i] = "<s>" + groceryItemString[i] + "</s>";
            }
            groceryList.text += groceryItemCount[i] + "x" + groceryItemString[i] + "\n";
        }
        CheckFinishedGame();
    }

    private void CheckFinishedGame()
    {
        if(groceryItemCount.All(x => x == 0))
        {
            groceryList.text = "YOU WON!";
        }
    }

    public void UpdateFontColour(int j)
    {
        groceryList.text = "";
        for (int i = 0; i < groceryItemCount.Length; i++)
        {
            if (i == j)
            {
                groceryItemString[i] = "<color=green>" + groceryItemString[i] + "</color>";
                itemHints[i].text = itemHintsText[i];
            }
            groceryList.text += groceryItemCount[i] + "x" + groceryItemString[i] + "\n";
        }
    }

    private void GameLost()
    {
        groceryList.text = "GAME OVER";
    }

    public void CloseUIElement(GameObject UIElement)
    {
        UIElement.SetActive(false);
    }

    public void OpenUIElement(GameObject UIElement)
    {
        UIElement.SetActive(true);
    }

    public void SetPluralConjugation()
    {
        pluralVerbage = !pluralVerbage;
    }

    public void SetGroupA()
    {
        genderNeutral = true;
    }

    public void SetGroupB()
    {
        genderNeutral = false;
    }

    public void DisableMove()
    {
        disableMove = true;
        geniesInputs.CursorLocked = true;
        starterInput.SetCursorForLookState(false);
        starterInput.SetCursorState(false);
    }

    public void EnableMove()
    {
        disableMove = false;
        geniesInputs.CursorLocked = false;
        starterInput.SetCursorForLookState(true);
        starterInput.SetCursorState(true);
    }
}
