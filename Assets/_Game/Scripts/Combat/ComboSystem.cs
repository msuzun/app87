using System.Collections.Generic;
using UnityEngine;

namespace NeonSyndicate.Combat
{
    /// <summary>
    /// Combo sistemini yöneten sınıf.
    /// ScriptableObject'ten combo verisini alır ve zinciri yönetir.
    /// </summary>
    public class ComboSystem : MonoBehaviour
    {
        [Header("Combo Settings")]
        [SerializeField] private ComboData comboData;
        [SerializeField] private float comboResetTime = 1f;

        [Header("Current Combo State")]
        [SerializeField] private int currentComboIndex = 0;
        [SerializeField] private float lastAttackTime;
        [SerializeField] private int totalComboHits = 0;

        private Animator animator;

        // Events
        public delegate void ComboEvent(int comboIndex);
        public event ComboEvent OnComboAdvanced;
        public event ComboEvent OnComboReset;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Combo timeout kontrolü
            if (Time.time - lastAttackTime > comboResetTime)
            {
                ResetCombo();
            }
        }

        /// <summary>
        /// Kombonun bir sonraki adımını çalıştırır.
        /// </summary>
        public void ExecuteNextAttack()
        {
            if (comboData == null || comboData.ComboSteps.Count == 0)
            {
                Debug.LogWarning("Combo Data is not assigned!");
                return;
            }

            // Combo adımını al
            ComboStep step = comboData.ComboSteps[currentComboIndex];

            // Animasyonu tetikle
            animator.SetTrigger(step.AnimationTrigger);

            // Hasar değerini kaydet (Hitbox bunu kullanacak)
            // Bu değer public property olarak erişilebilir

            // Combo ilerlet
            totalComboHits++;
            lastAttackTime = Time.time;

            OnComboAdvanced?.Invoke(currentComboIndex);

            // Sonraki adıma geç
            currentComboIndex = (currentComboIndex + 1) % comboData.ComboSteps.Count;
        }

        /// <summary>
        /// Kombonun devam edip edemeyeceğini kontrol eder.
        /// </summary>
        public bool CanContinueCombo()
        {
            if (comboData == null) return false;

            float timeSinceLastAttack = Time.time - lastAttackTime;
            ComboStep currentStep = comboData.ComboSteps[currentComboIndex == 0 ? comboData.ComboSteps.Count - 1 : currentComboIndex - 1];

            return timeSinceLastAttack <= currentStep.CancelWindow;
        }

        /// <summary>
        /// Komboyu sıfırlar.
        /// </summary>
        public void ResetCombo()
        {
            if (currentComboIndex != 0)
            {
                currentComboIndex = 0;
                OnComboReset?.Invoke(0);
            }
        }

        /// <summary>
        /// Mevcut combo adımının hasar değerini döner.
        /// </summary>
        public float GetCurrentDamage()
        {
            if (comboData == null || comboData.ComboSteps.Count == 0)
                return 10f;

            int index = currentComboIndex == 0 ? comboData.ComboSteps.Count - 1 : currentComboIndex - 1;
            return comboData.ComboSteps[index].Damage;
        }

        #region Properties
        public int CurrentComboIndex => currentComboIndex;
        public int TotalComboHits => totalComboHits;
        public ComboData ComboData => comboData;
        #endregion
    }

    /// <summary>
    /// Combo'nun tek bir adımını temsil eder.
    /// </summary>
    [System.Serializable]
    public class ComboStep
    {
        [Header("Animation")]
        public string AnimationTrigger = "Attack1";

        [Header("Damage")]
        public float Damage = 10f;
        public float KnockbackForce = 5f;

        [Header("Timing")]
        [Tooltip("Bu saldırıdan sonraki komboyu iptal edebileceğin süre")]
        public float CancelWindow = 0.5f;

        [Header("Properties")]
        public bool IsLauncher = false; // Düşmanı havaya kaldırır mı?
        public bool IsFinisher = false;  // Combo'nun son vuruşu mu?
    }
}

