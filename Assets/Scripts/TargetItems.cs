using UnityEngine;

public class TargetItems : MonoBehaviour
{
    public int id;

    private void Start()
    {
        foreach(Transform child in this.transform)
        {
            child.GetComponent<Item>().itemId = id;
        }
    }

    public void Validate()
    {
        foreach(Transform child in this.transform)
        {
            child.GetComponent<Item>().validated = true;
        }
    }

   
}
