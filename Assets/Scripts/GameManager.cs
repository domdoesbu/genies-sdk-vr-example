using Meta.XR.Movement;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1. Unlock NPC
    // 2. Unlock item associated with NPC
    // 3. Check items being put in basket
    // 4. If item is not unlocked, it is invalid
    //    If item is incorrect, it is invalid
    // 5. If item is unlocked and correct, it is valid, and that is checked off the list.

    // 2D array of NPC and item

    /** LIST:
        - 1 bundt cake
        - 3 Green grapes
        - 5 apples
        - 2 meat
        - 1 rubber duck
    **/

    [SerializeField] public int[] groceryItemCount = new int[5];
    [SerializeField] public TextMeshProUGUI groceryList;
    private string[] groceryItemString = new string[5];

    public Health health;

    public bool genderNeutral = false;
    public bool pluralVerbage = false;

    public GameObject[] mirrors = new GameObject[4];

    private void Start()
    { 
        groceryItemString[0] = " asepoifj";
        groceryItemString[1] = " asrgha ";
        groceryItemString[2] = " rvasrg";
        groceryItemString[3] = " afvcd";
        groceryItemString[4] = " aduhafga";

        groceryList.text = "";
        for (int i = 0; i < groceryItemCount.Length; i++)
        {
            groceryList.text += groceryItemCount[i] + groceryItemString[i] + "\n";
        }
    }

    public void ToggleOutsideMirror()
    {
        mirrors[0].gameObject.SetActive(false);
    }

    public void ToggleInsideMirror()
    {
        for(int i = 1; i < mirrors.Length; i++)
        {
            mirrors[i].gameObject.SetActive(!mirrors[i].gameObject.activeSelf);
        }
    }

    public void DecreaseHealth()
    {
        health.DecreaseHealth();
        if(health.healthCount == 0)
        {
            GameEnd();
        }
    }

    public void UpdateList()
    {
        groceryList.text = "";
        for (int i = 0; i < groceryItemCount.Length; i++)
        {
            if (groceryItemCount[i] == 0)
            {
                Debug.Log("i :  " + groceryItemCount[i]);
                groceryItemString[i] = "<s>" + groceryItemString[i] + "</s>";
            }
            groceryList.text += groceryItemCount[i] + groceryItemString[i] + "\n";
        }
    }

    public void GameEnd()
    {
        groceryList.text = "GAME OVER";
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
}
