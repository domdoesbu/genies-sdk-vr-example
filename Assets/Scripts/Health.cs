using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public GameObject[] health;
    public int healthCount;

    public void DecreaseHealth()
    {
        Destroy(health[healthCount - 1]);
        healthCount -= 1;

    }
}
