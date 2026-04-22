using UnityEngine;

public class TargetItems : MonoBehaviour
{
    public int id;
    public GameManager manager;
    private void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        foreach(Transform child in this.transform)
        {
            child.GetComponent<Item>().itemId = id;
        }
    }

    public void Validate()
    {
        manager.UpdateFontColour(id);
        foreach(Transform child in this.transform)
        {
            child.GetComponent<Item>().validated = true;
        }
    }

   
}
