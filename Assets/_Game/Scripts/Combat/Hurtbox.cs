using UnityEngine;

namespace NeonSyndicate.Combat
{
    /// <summary>
    /// Hasar alan bölge (karakterin gövdesi).
    /// CharacterBase ile entegre çalışır.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hurtbox : MonoBehaviour
    {
        [Header("Hurtbox Settings")]
        [SerializeField] private bool showDebugGizmo = true;
        [SerializeField] private Color gizmoColor = Color.green;

        private Collider2D hurtboxCollider;

        private void Awake()
        {
            hurtboxCollider = GetComponent<Collider2D>();
            hurtboxCollider.isTrigger = false; // Hurtbox solid collider'dır
        }

        /// <summary>
        /// Hurtbox'ı aktif/deaktif eder (ölüm veya invulnerability için).
        /// </summary>
        public void SetActive(bool active)
        {
            hurtboxCollider.enabled = active;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmo) return;

            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = gizmoColor;
                
                if (col is BoxCollider2D boxCol)
                {
                    Gizmos.DrawWireCube(transform.position + (Vector3)boxCol.offset, boxCol.size);
                }
                else if (col is CircleCollider2D circleCol)
                {
                    Gizmos.DrawWireSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
                }
            }
        }
    }
}

