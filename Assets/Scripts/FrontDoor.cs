using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
public class FrontDoor : MonoBehaviour
{
    [SerializeField] public GameObject FrontDoorPrefab;
    [SerializeField] public GameObject debugger;
    public bool open = false;
    public bool close = false;
    public Vector3 pStart;
    public Vector3 pEnd;
    public TextMeshProUGUI debugText;

    int interpolationFramesCount = 100;
    int elapsedFrames = 0;
    int maxFrameReset = 900;

    void Start()
    {

        pStart = FrontDoorPrefab.transform.position;
        pEnd = new Vector3(pStart.x - 1f, pStart.y, pStart.z);
        debugText = debugger.GetComponent<TMPro.TextMeshProUGUI>();
        Debug.Log( "pStart: " + pStart + " :: pEnd: " + pEnd);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (open)
        { 
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            FrontDoorPrefab.transform.position = Vector3.Lerp(FrontDoorPrefab.transform.position, pEnd, interpolationRatio);
            Debug.Log("open ");
            elapsedFrames = (elapsedFrames + 1) % (maxFrameReset);

        }
        if (close)
        {
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            FrontDoorPrefab.transform.position = Vector3.Lerp(FrontDoorPrefab.transform.position, pStart, interpolationRatio);
            Debug.Log("close ");
            elapsedFrames = (elapsedFrames + 1) % (maxFrameReset);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag.Equals("Player"))
        {
            elapsedFrames = 0;
            Debug.Log("Trigger enter");
            open = true;
            close = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.tag.Equals("Player"))
        {
            elapsedFrames = 0;
            Debug.Log("Trigger exit");
            open = false;
            close = true;
        }
    }


}
