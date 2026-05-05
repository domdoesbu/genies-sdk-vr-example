using UnityEngine;

public class Basket : MonoBehaviour
{
    private GameManager gameManager;
    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Pickable is layer 12
        if(other.gameObject.layer == 12)
        {
            Item item = other.gameObject.GetComponent<Item>();
            
            if(item != null && item.validated)
            {
                if (gameManager.groceryItemCount[item.itemId] == 0)
                {
                    Debug.Log("Invalid item");
                    gameManager.DecreaseHealth();
                }
                else
                {
                    gameManager.groceryItemCount[item.itemId] -= 1;
                    Debug.Log("Correct item: " + gameManager.groceryItemCount[item.itemId]);
                }
                gameManager.UpdateList();
            }
            else
            {
                gameManager.DecreaseHealth();
                Debug.Log("Incorrect item");
            }
            Destroy(other.gameObject);
        }
    }
}
