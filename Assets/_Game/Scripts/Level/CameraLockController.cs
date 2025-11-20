using UnityEngine;
using NeonSyndicate.Utils;

namespace NeonSyndicate.Level
{
    /// <summary>
    /// Camera Lock Controller - Wave sırasında kamerayı kilitler.
    /// CameraFollow script'i ile entegre çalışır.
    /// 
    /// Beat 'em up mekanizması:
    /// - Wave başladı → Kamera kilitleniyor
    /// - Görünmez duvarlar spawn oluyor
    /// - Düşmanlar öldü → Kamera serbest
    /// - Görünmez duvarlar kaldırılıyor
    /// </summary>
    public class CameraLockController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private Transform invisibleWallLeft;
        [SerializeField] private Transform invisibleWallRight;

        [Header("Lock Settings")]
        [SerializeField] private float lockTransitionSpeed = 2f;
        [SerializeField] private float wallSpawnDistance = 10f; // Kameradan ne kadar uzakta duvar

        [Header("Boundaries")]
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        [Header("State")]
        [SerializeField] private bool isLocked = false;
        [SerializeField] private float targetLockX = 0f;

        private Vector3 originalCameraPosition;

        private void Awake()
        {
            if (cameraFollow == null)
            {
                cameraFollow = Camera.main.GetComponent<CameraFollow>();
            }

            // Görünmez duvarları başlangıçta deaktif et
            if (invisibleWallLeft != null) invisibleWallLeft.gameObject.SetActive(false);
            if (invisibleWallRight != null) invisibleWallRight.gameObject.SetActive(false);
        }

        /// <summary>
        /// Kamerayı belirli bir X pozisyonunda kilitler.
        /// </summary>
        public void LockCamera(float lockPositionX)
        {
            isLocked = true;
            targetLockX = lockPositionX;
            originalCameraPosition = Camera.main.transform.position;

            // CameraFollow'u durdur
            if (cameraFollow != null)
            {
                cameraFollow.enabled = false;
            }

            // Kamerayı target pozisyona smooth transition
            StartCoroutine(TransitionToLockPosition());

            // Görünmez duvarları spawn et
            SpawnInvisibleWalls(lockPositionX);

            Debug.Log($"[CameraLock] Camera locked at X: {lockPositionX}");
        }

        /// <summary>
        /// Kamera kilidini açar.
        /// </summary>
        public void UnlockCamera()
        {
            isLocked = false;

            // CameraFollow'u tekrar aktif et
            if (cameraFollow != null)
            {
                cameraFollow.enabled = true;
            }

            // Duvarları kaldır
            RemoveInvisibleWalls();

            Debug.Log("[CameraLock] Camera unlocked");
        }

        /// <summary>
        /// Kamera smooth geçiş yapar.
        /// </summary>
        private IEnumerator TransitionToLockPosition()
        {
            Vector3 targetPosition = new Vector3(targetLockX, Camera.main.transform.position.y, Camera.main.transform.position.z);

            while (Vector3.Distance(Camera.main.transform.position, targetPosition) > 0.1f)
            {
                Camera.main.transform.position = Vector3.Lerp(
                    Camera.main.transform.position,
                    targetPosition,
                    lockTransitionSpeed * Time.deltaTime
                );

                yield return null;
            }

            Camera.main.transform.position = targetPosition;
        }

        /// <summary>
        /// Görünmez duvarları spawn eder.
        /// </summary>
        private void SpawnInvisibleWalls(float centerX)
        {
            if (invisibleWallLeft != null)
            {
                invisibleWallLeft.position = new Vector3(centerX - wallSpawnDistance, 0, 0);
                invisibleWallLeft.gameObject.SetActive(true);
            }

            if (invisibleWallRight != null)
            {
                invisibleWallRight.position = new Vector3(centerX + wallSpawnDistance, 0, 0);
                invisibleWallRight.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Görünmez duvarları kaldırır.
        /// </summary>
        private void RemoveInvisibleWalls()
        {
            if (invisibleWallLeft != null) invisibleWallLeft.gameObject.SetActive(false);
            if (invisibleWallRight != null) invisibleWallRight.gameObject.SetActive(false);
        }

        /// <summary>
        /// Level başlangıcında kamera sınırlarını ayarla.
        /// </summary>
        public void SetBoundaries(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;

            if (cameraFollow != null)
            {
                cameraFollow.SetBoundaries(min, max);
            }
        }

        private void OnDrawGizmos()
        {
            // Kamera lock pozisyonunu göster
            if (isLocked)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3(targetLockX, minBounds.y, 0),
                    new Vector3(targetLockX, maxBounds.y, 0)
                );

                // Duvar pozisyonları
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(
                    new Vector3(targetLockX - wallSpawnDistance, 0, 0),
                    new Vector3(0.5f, 10f, 1f)
                );
                Gizmos.DrawWireCube(
                    new Vector3(targetLockX + wallSpawnDistance, 0, 0),
                    new Vector3(0.5f, 10f, 1f)
                );
            }
        }
    }
}

