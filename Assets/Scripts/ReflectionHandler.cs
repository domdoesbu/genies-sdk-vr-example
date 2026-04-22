using UnityEngine;

public class ReflectionHandler : MonoBehaviour
{
    [SerializeField] public GameObject cube;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            cube.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            cube.SetActive(true);
        }
    }
}
