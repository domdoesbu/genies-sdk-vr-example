using TMPro;
using UnityEngine;

public class DialogueVariables : MonoBehaviour
{

    // GUI elements
    [Header("TMP Objects")]
    public GameObject pronounParent;

    public TextMeshProUGUI playerNameInput;
    public TextMeshProUGUI subPronounInput;
    public TextMeshProUGUI objPronounInput;
    public TextMeshProUGUI possAdjInput;
    public TextMeshProUGUI possPronounInput;
    public TextMeshProUGUI reflexPronounInput;

    public TMP_Dropdown genderedTermInput;
    public TMP_Dropdown sassyInput;
    public TMP_Dropdown politeInput;

    [Header("Pronoun values")]
    // Name
    public string playerName;

    // Pronouns
    public string subjectivePronoun;       // he, she, it, they
    public string subjectivePronounCaps;
    public string objectivePronoun;        // him, her, it, them
    public string possesiveAdjectives;  // his, her, its, their
    public string possesivePronoun;     // his, hers, its, theirs
    public string reflexivePronoun;     // himself, herself, itself, themselves 

    // Gendered language
    public string genderedTerm;         // guy, girl, person
    public string sassy;                // dude, babe, buddy
    public string polite;               // sir, ma'am, friend

    public string beVerb;
    public string BeVerb;
    public string haveVerb;
    public string doVerb;
    public string seekVerb;
    public void SetPronouns()
    {
        playerName = playerNameInput.text;
        subjectivePronoun = subPronounInput.text;
        objectivePronoun = objPronounInput.text;
        possesiveAdjectives = possAdjInput.text;
        possesivePronoun = possPronounInput.text;
        reflexivePronoun = reflexPronounInput.text;

        pronounParent.SetActive(false);
    }

    public void UpdateDropDown(int i)
    {
        if(i == 1)
        {
            Debug.Log(genderedTermInput.options[genderedTermInput.value].text);
            genderedTerm = genderedTermInput.options[genderedTermInput.value].text.ToLower();
        }
        if (i == 2) 
        {
            sassy = sassyInput.options[sassyInput.value].text.ToLower();
        }
        if (i == 3)
        {
            polite = politeInput.options[politeInput.value].text.ToLower();
        }
    }
}
