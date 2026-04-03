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
    void Start()
    {

        pStart = FrontDoorPrefab.transform.position;
        pEnd = new Vector3(pStart.x - 1f, pStart.y, pStart.z);
        debugText = debugger.GetComponent<TMPro.TextMeshProUGUI>();
        debugText.text = "pStart: " + pStart + " :: pEnd: " + pEnd;
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "open: " + open + " :: close: " + close;
        if (open)
        {
            FrontDoorPrefab.transform.position = Vector3.Lerp(pEnd, pStart, Time.deltaTime * 0.0002f);
        }
        if (close)
        {
            FrontDoorPrefab.transform.position = Vector3.Lerp(pStart, pEnd, Time.deltaTime * 0.0002f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            open = true;
            close = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            open = false;
            close = true;
        }
    }


}
