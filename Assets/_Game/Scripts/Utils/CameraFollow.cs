using UnityEngine;

namespace NeonSyndicate.Utils
{
    /// <summary>
    /// Kamerayı oyuncuyu takip ettiren basit bir script.
    /// Beat 'em up tarzı side-scrolling için optimize edilmiştir.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;

        [Header("Boundaries")]
        [SerializeField] private bool useBoundaries = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-10, -5);
        [SerializeField] private Vector2 maxBounds = new Vector2(10, 5);

        private void LateUpdate()
        {
            if (target == null) return;

            // Hedef pozisyonu hesapla
            Vector3 desiredPosition = target.position + offset;

            // Hangi eksenleri takip edeceğimize karar ver
            if (!followX)
                desiredPosition.x = transform.position.x;
            if (!followY)
                desiredPosition.y = transform.position.y;

            // Boundary kontrolü
            if (useBoundaries)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }

            // Smooth takip
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Hedefi değiştirir (örn: boss battle başladığında).
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Kamera sınırlarını değiştirir (yeni level'a geçildiğinde).
        /// </summary>
        public void SetBoundaries(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
        }

        private void OnDrawGizmosSelected()
        {
            if (!useBoundaries) return;

            // Kamera sınırlarını gizmo ile göster
            Gizmos.color = Color.yellow;
            Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0);
            Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0);
            Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0);
            Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
}

