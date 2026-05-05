using UnityEngine;
using Oculus.Interaction;
public class ChangeGrabLayers : MonoBehaviour
{
    private int grabLayer = 12;
    private Grabbable grabbable;
    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
    }

    private void OnEnable()
    {
        grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }
    private void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select) 
        {
            gameObject.layer = grabLayer;
        }
    }

}
