using UnityEngine;

namespace Genies.VRExample
{
    public class MatchYPosition : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothFactor = 5f;
        public float offset = 0f;

        // Only adjust Y when the target is farther than this many units away on Y.
        [SerializeField, Min(0f)] private float yThreshold = 0.05f;

        private void Update()
        {
            if (target == null)
                return;

            Vector3 currentPosition = transform.position;
            float targetY = target.position.y;

            if (Mathf.Abs(targetY - currentPosition.y) <= yThreshold)
                return;

            float newY = Mathf.Lerp(
                currentPosition.y,
                targetY + offset,
                smoothFactor * Time.deltaTime
            );

            transform.position = new Vector3(
                currentPosition.x,
                newY,
                currentPosition.z
            );
        }
    }
}
