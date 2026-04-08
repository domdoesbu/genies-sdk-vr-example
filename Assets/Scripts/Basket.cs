using UnityEngine;

public class Basket : MonoBehaviour
{

    // Layer 6: Valid
    // Layer 7: Invalid

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            
        }
    }


}
