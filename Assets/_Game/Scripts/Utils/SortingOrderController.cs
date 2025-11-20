using UnityEngine;

namespace NeonSyndicate.Utils
{
    /// <summary>
    /// 2.5D derinlik için Y eksenine göre sprite sorting order'ı ayarlar.
    /// Crazy Flasher'daki gibi derinlik hissi yaratır.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SortingOrderController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool autoUpdate = true;
        [SerializeField] private float sortingOrderMultiplier = 100f;
        [SerializeField] private string sortingLayerName = "Characters";

        private SpriteRenderer spriteRenderer;
        private Transform cachedTransform;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            cachedTransform = transform;

            // Sorting layer'ı ayarla
            if (!string.IsNullOrEmpty(sortingLayerName))
            {
                spriteRenderer.sortingLayerName = sortingLayerName;
            }
        }

        private void LateUpdate()
        {
            if (autoUpdate)
            {
                UpdateSortingOrder();
            }
        }

        /// <summary>
        /// Y pozisyonuna göre sorting order'ı günceller.
        /// Y değeri ne kadar düşükse (aşağıda), sprite o kadar öndedir.
        /// </summary>
        public void UpdateSortingOrder()
        {
            // Negatif Y değeri = daha önde (daha yüksek sorting order)
            int newOrder = Mathf.RoundToInt(-cachedTransform.position.y * sortingOrderMultiplier);
            spriteRenderer.sortingOrder = newOrder;
        }

        /// <summary>
        /// Manuel olarak sorting order ayarlar.
        /// </summary>
        public void SetSortingOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}

