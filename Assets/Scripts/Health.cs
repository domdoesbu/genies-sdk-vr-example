using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public GameObject[] health;
    public int healthCount;

    public void DecreaseHealth()
    {
        if(healthCount > 0)
        {
            Destroy(health[healthCount - 1]);
            healthCount -= 1;
        }
        

    }
}
