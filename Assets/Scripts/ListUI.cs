using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ListUI : MonoBehaviour
{
    public GameObject HUD;
    public StarterAssetsInputs _input;
    public bool hidden = false;
    public Vector3 originalScale;
    public GameManager manager;
    private void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        originalScale = HUD.transform.localScale;
   
    }
    private void Update()
    {
        if (_input == null)
        {
            if(OVRInput.GetDown(OVRInput.RawButton.X) && hidden)
            {
                HUD.transform.localScale = originalScale;
                hidden = false;
            }
            else if(OVRInput.GetDown(OVRInput.RawButton.X) && !hidden)
            {
                HUD.transform.localScale = Vector3.zero;
                hidden = true;
            }
        }
        else if(_input != null) 
        {
            if (_input.toggleHUD)
            { 
                HUD.transform.localScale = Vector3.zero;
                hidden = false;
            }   
            else if(!_input.toggleHUD)
            {
                HUD.transform.localScale = originalScale;
                hidden = true;
            }
        }
    }
}
