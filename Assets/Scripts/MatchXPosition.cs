using UnityEngine;

namespace Genies.VRExample
{
    public class MatchXPosition : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothFactor = 5f;

        // Only adjust X when the target is farther than this many units away on X.
        [SerializeField, Min(0f)] private float xThreshold = 0.05f;

        private void Update()
        {
            if (target == null)
                return;

            Vector3 currentPosition = transform.position;
            float targetX = target.position.x;

            if (Mathf.Abs(targetX - currentPosition.x) <= xThreshold)
                return;

            float newX = Mathf.Lerp(
                targetX,
                currentPosition.x,
                smoothFactor * Time.deltaTime
            );

            transform.position = new Vector3(
                newX,
                currentPosition.y,
                currentPosition.z
            );
        }
    }
}
