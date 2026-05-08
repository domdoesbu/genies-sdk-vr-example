
using UnityEngine;

public class ChangeGrabbedLayer : MonoBehaviour
{
    private int grabLayer = 15;

    public void Grabbed()
    {
        gameObject.layer = grabLayer;
    }

}
