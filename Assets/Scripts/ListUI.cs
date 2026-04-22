using UnityEngine;

public class ListUI : MonoBehaviour
{
    public GameObject HUD;
    public bool hidden = false;
    public Vector3 originalScale;

    private void Start()
    {
        originalScale = HUD.transform.localScale;
    }
    private void Update()
    {
        
        if (OVRInput.GetDown(OVRInput.RawButton.X) && !hidden)
        {
            HUD.transform.localScale = Vector3.zero;
            hidden = true;
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.X) && hidden)
        {
            HUD.transform.localScale = originalScale;
            hidden = false;
        }
    }
}
