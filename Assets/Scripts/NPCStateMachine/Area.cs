using UnityEngine;
using UnityEngine.AI;

public class Area : MonoBehaviour
{
    public float radius = 20f;
    public float width = 2f;
    public float length = 3f;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0.1f, length));
    }

    public Vector3 GetRandomPoint()
    {
        float randomX = Random.Range(-width / 2f, width / 2f);
        float randomZ = Random.Range(-length / 2f, length / 2f);

        Vector3 randomPoint = transform.position + new Vector3(randomX, 0f, randomZ);

        NavMeshHit hit;
        Vector3 finalPosition = transform.position;

        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
}
