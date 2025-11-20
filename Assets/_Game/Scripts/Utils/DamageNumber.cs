using UnityEngine;
using TMPro;

namespace NeonSyndicate.Utils
{
    /// <summary>
    /// Vuruş anında ekranda çıkan hasar sayısı (Floating Damage Text).
    /// Object Pooler ile kullanılmak üzere tasarlanmıştır.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageNumber : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float floatSpeed = 2f;
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private TextMeshPro textMesh;
        private float timer;
        private Vector3 startPosition;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        /// <summary>
        /// Hasar sayısını başlatır (Object Pooler'dan spawn edildiğinde çağrılır).
        /// </summary>
        public void Initialize(float damage, Vector3 position, bool isCritical = false)
        {
            startPosition = position;
            transform.position = position;
            timer = 0f;

            // Hasar metnini ayarla
            textMesh.text = Mathf.RoundToInt(damage).ToString();

            // Kritik vuruşsa farklı renk/boyut
            if (isCritical)
            {
                textMesh.color = Color.red;
                textMesh.fontSize = 6f;
                textMesh.text += "!";
            }
            else
            {
                textMesh.color = Color.white;
                textMesh.fontSize = 4f;
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;

            // Yukarı doğru float
            transform.position = startPosition + Vector3.up * (timer * floatSpeed);

            // Scale animasyonu
            float scaleProgress = timer / lifetime;
            float scale = scaleCurve.Evaluate(scaleProgress);
            transform.localScale = Vector3.one * scale;

            // Alpha fade
            Color color = textMesh.color;
            color.a = 1f - scaleProgress;
            textMesh.color = color;

            // Ömrü dolduysa yok et
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}

