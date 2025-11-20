using UnityEngine;

namespace NeonSyndicate.Environment
{
    /// <summary>
    /// Parallax Background - Derinlik hissi yaratır.
    /// 
    /// Kullanım:
    /// - Background layer: Slow movement (0.1x)
    /// - Midground layer: Medium movement (0.5x)
    /// - Foreground layer: Fast movement (1.2x)
    /// 
    /// Crazy Flasher'daki atmosferik derinlik hissi için kritik!
    /// </summary>
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Parallax hızı çarpanı (0 = statik, 1 = kamera ile aynı hız)")]
        [SerializeField] private float parallaxMultiplier = 0.5f;
        
        [Tooltip("Sadece X ekseninde parallax yap")]
        [SerializeField] private bool lockY = true;
        
        [Tooltip("Looping (tekrarlama) kullan")]
        [SerializeField] private bool useLooping = false;
        
        [Tooltip("Sprite genişliği (looping için)")]
        [SerializeField] private float spriteWidth = 20f;

        [Header("Auto-Scroll (Metro seviyesi için)")]
        [Tooltip("Otomatik scroll kullan (tren/metro için)")]
        [SerializeField] private bool autoScroll = false;
        
        [Tooltip("Auto-scroll hızı")]
        [SerializeField] private float scrollSpeed = 2f;

        private Transform cameraTransform;
        private Vector3 previousCameraPosition;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            previousCameraPosition = cameraTransform.position;
        }

        private void LateUpdate()
        {
            if (cameraTransform == null) return;

            if (autoScroll)
            {
                AutoScrollUpdate();
            }
            else
            {
                ParallaxUpdate();
            }
        }

        /// <summary>
        /// Normal parallax hareketi.
        /// </summary>
        private void ParallaxUpdate()
        {
            // Kamera ne kadar hareket etti?
            Vector3 cameraDelta = cameraTransform.position - previousCameraPosition;

            // Parallax hareketi uygula
            float parallaxX = cameraDelta.x * parallaxMultiplier;
            float parallaxY = lockY ? 0 : cameraDelta.y * parallaxMultiplier;

            transform.position += new Vector3(parallaxX, parallaxY, 0);

            // Looping kontrolü
            if (useLooping)
            {
                CheckLooping();
            }

            previousCameraPosition = cameraTransform.position;
        }

        /// <summary>
        /// Auto-scroll (tren/metro sahnesi için).
        /// </summary>
        private void AutoScrollUpdate()
        {
            // Sürekli sola scroll
            transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // Looping
            if (useLooping)
            {
                CheckLooping();
            }
        }

        /// <summary>
        /// Sprite sınırın dışına çıktıysa tekrar başa sar.
        /// </summary>
        private void CheckLooping()
        {
            float cameraX = cameraTransform.position.x;
            float bgX = transform.position.x;

            // Sağdan kayboldu mu? → Sola taşı
            if (bgX < cameraX - spriteWidth)
            {
                transform.position += new Vector3(spriteWidth * 2f, 0, 0);
            }
            // Soldan kayboldu mu? → Sağa taşı
            else if (bgX > cameraX + spriteWidth)
            {
                transform.position -= new Vector3(spriteWidth * 2f, 0, 0);
            }
        }
    }
}

