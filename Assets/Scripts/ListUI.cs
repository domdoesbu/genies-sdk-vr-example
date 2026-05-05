using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ListUI : MonoBehaviour
{
    public GameObject HUD;
    public GameObject hintList;
    public StarterAssetsInputs _input;
    public bool listHidden = false;
    public bool hintHidden = true;
    public Vector3 listOriginalScale;
    public GameManager manager;
    private void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        listOriginalScale = HUD.transform.localScale;
   
    }
    private void Update()
    {
        if (_input == null)
        {
            if(OVRInput.GetDown(OVRInput.RawButton.X) && listHidden)
            {
                HUD.transform.localScale = listOriginalScale;
                listHidden = false;
            }
            else if(OVRInput.GetDown(OVRInput.RawButton.X) && !listHidden)
            {
                HUD.transform.localScale = Vector3.zero;
                listHidden = true;
            }
            if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                hintHidden = !hintHidden;
                hintList.SetActive(hintHidden);
            }
        }
        else if(_input != null && !manager.disableMove) 
        {
            if (_input.toggleHUD)
            { 
                HUD.transform.localScale = Vector3.zero;
                listHidden = false;
            }   
            else if(!_input.toggleHUD)
            {
                HUD.transform.localScale = listOriginalScale;
                listHidden = true;
            }
        }
    }
}
