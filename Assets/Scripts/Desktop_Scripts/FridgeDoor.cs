using Unity.VisualScripting;
using UnityEngine;

public class FridgeDoor : MonoBehaviour
{
    int interpolationFramesCount = 100;
    int elapsedFrames = 0;
    int maxFrameReset = 900;
    public bool interact;
    public float closedAngle;
    public float openAngle;
    public bool isClosed = true;
    private Vector3 closed;
    private Vector3 open;
    public float speed = 2f;
    private Quaternion targetRotation;

    private void Start()
    {
        closed = new Vector3(0f, closedAngle, 0f);
        open = new Vector3(0f, openAngle, 0f);
        targetRotation = Quaternion.Euler(closed);
    }

    void Update()
    {
        if (interact)
        {
            targetRotation = isClosed ? Quaternion.Euler(closed) : Quaternion.Euler(open);

            interact = false;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);
    }
}
