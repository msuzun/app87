using UnityEngine;

namespace NeonSyndicate.Utils
{
    /// <summary>
    /// Crazy Flasher tarzı Ragdoll fiziği kontrolcüsü.
    /// Karakter öldüğünde bez bebek gibi yere yığılmasını sağlar.
    /// </summary>
    public class RagdollController : MonoBehaviour
    {
        [Header("Ragdoll Parts")]
        [SerializeField] private Rigidbody2D[] limbRigidbodies;
        [SerializeField] private Collider2D[] limbColliders;

        [Header("Settings")]
        [SerializeField] private bool isRagdollActive = false;
        [SerializeField] private float angularDrag = 2f;
        [SerializeField] private float linearDrag = 1f;

        private Animator animator;
        private Rigidbody2D mainRigidbody;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mainRigidbody = GetComponent<Rigidbody2D>();

            // Başlangıçta ragdoll'u deaktif et
            SetRagdollActive(false);
        }

        /// <summary>
        /// Ragdoll'u aktifleştirir (karakter öldüğünde çağrılır).
        /// </summary>
        public void ActivateRagdoll()
        {
            if (isRagdollActive) return;

            SetRagdollActive(true);

            // Animator'ı kapat
            if (animator != null)
            {
                animator.enabled = false;
            }

            // Ana rigidbody'yi deaktif et
            if (mainRigidbody != null)
            {
                mainRigidbody.simulated = false;
            }
        }

        /// <summary>
        /// Ragdoll'u deaktif eder (karakter hayata dönerse).
        /// </summary>
        public void DeactivateRagdoll()
        {
            if (!isRagdollActive) return;

            SetRagdollActive(false);

            // Animator'ı aç
            if (animator != null)
            {
                animator.enabled = true;
            }

            // Ana rigidbody'yi aktif et
            if (mainRigidbody != null)
            {
                mainRigidbody.simulated = true;
            }
        }

        private void SetRagdollActive(bool active)
        {
            isRagdollActive = active;

            // Tüm limb rigidbody'leri ayarla
            foreach (Rigidbody2D rb in limbRigidbodies)
            {
                if (rb != null)
                {
                    rb.simulated = active;
                    rb.bodyType = active ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

                    if (active)
                    {
                        rb.angularDrag = angularDrag;
                        rb.drag = linearDrag;
                    }
                }
            }

            // Tüm limb collider'ları ayarla
            foreach (Collider2D col in limbColliders)
            {
                if (col != null)
                {
                    col.enabled = active;
                }
            }
        }

        /// <summary>
        /// Ragdoll'a kuvvet uygular (ölüm anında fırlatma için).
        /// </summary>
        public void ApplyForceToRagdoll(Vector2 force)
        {
            if (!isRagdollActive) return;

            foreach (Rigidbody2D rb in limbRigidbodies)
            {
                if (rb != null)
                {
                    rb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }

        #region Editor Helper
        /// <summary>
        /// Inspector'da "Setup Ragdoll" butonu için.
        /// Tüm child Rigidbody2D ve Collider2D'leri otomatik bulur.
        /// </summary>
        [ContextMenu("Auto-Setup Ragdoll Parts")]
        private void AutoSetupRagdollParts()
        {
            limbRigidbodies = GetComponentsInChildren<Rigidbody2D>();
            limbColliders = GetComponentsInChildren<Collider2D>();

            Debug.Log($"Found {limbRigidbodies.Length} rigidbodies and {limbColliders.Length} colliders.");
        }
        #endregion
    }
}

