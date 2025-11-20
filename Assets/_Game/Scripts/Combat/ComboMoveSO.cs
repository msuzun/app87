using UnityEngine;
using System.Collections.Generic;

namespace NeonSyndicate.Combat
{
    /// <summary>
    /// Input tipleri - Kombo dallanması için.
    /// </summary>
    public enum InputType 
    { 
        None, 
        Light,      // Z tuşu (hızlı saldırı)
        Heavy,      // X tuşu (ağır saldırı)
        Special,    // C tuşu (özel saldırı)
        Jump        // Space (havada kombo için)
    }

    /// <summary>
    /// Data-Driven Combo System için ScriptableObject.
    /// Her saldırı hareketi bu asset ile tanımlanır.
    /// Unity Editor'de Create > Combat > Combo Move ile oluşturulur.
    /// </summary>
    [CreateAssetMenu(fileName = "New Combo Move", menuName = "Neon Syndicate/Combat/Combo Move")]
    public class ComboMoveSO : ScriptableObject
    {
        [Header("Animation & Visuals")]
        [Tooltip("Animator'daki State ismi (örn: 'Punch1', 'Kick2')")]
        public string animationName;
        
        [Tooltip("Animasyon süresi (saniye). Animator'da ayarlanan süre ile eşleşmeli.")]
        public float animationLength = 0.5f;
        
        [Tooltip("Vuruş efekti tipi (blood, spark, dust vb.)")]
        public string hitEffectName = "HitSpark";

        [Header("Combat Data")]
        [Tooltip("Bu saldırının hasar miktarı")]
        public float damage = 10f;
        
        [Tooltip("Knockback yönü ve gücü (x, y)")]
        public Vector2 knockback = new Vector2(5f, 2f);
        
        [Tooltip("Vuruş anında oyunun kaç saniye donacağı (Hit Stop)")]
        [Range(0f, 0.3f)]
        public float hitStopDuration = 0.1f;
        
        [Tooltip("Bu saldırı düşmanı havaya kaldırır mı? (Launcher)")]
        public bool isLauncher = false;
        
        [Tooltip("Bu kombo'nun son vuruşu mu? (Finisher)")]
        public bool isFinisher = false;

        [Header("Timing (Cancel Windows)")]
        [Tooltip("Kombo yapmak için en erken ne zaman tuşa basılabilir? (animasyon % cinsinden 0-1)")]
        [Range(0f, 1f)]
        public float minCancelTime = 0.3f;
        
        [Tooltip("Kombo yapmak için en geç ne zaman tuşa basılabilir? (animasyon % cinsinden 0-1)")]
        [Range(0f, 1f)]
        public float maxCancelTime = 0.8f;

        [Header("Movement")]
        [Tooltip("Saldırı sırasında karakterin ileri hareketi (momentum)")]
        public float forwardMomentum = 2f;
        
        [Tooltip("Bu saldırı havada yapılabilir mi?")]
        public bool canUseInAir = false;

        [Header("Combo Tree (Branching)")]
        [Tooltip("Bu hareketten sonra hangi hareketler gelebilir?")]
        public List<ComboBranch> nextMoves = new List<ComboBranch>();

        [Header("Audio")]
        [Tooltip("Saldırı ses efekti")]
        public string attackSoundName = "Whoosh_Attack";
        
        [Tooltip("Vuruş ses efekti")]
        public string hitSoundName = "Hit_Impact";

        [Header("Properties")]
        [Tooltip("Bu saldırı super armor sağlar mı? (Flinch etmeme)")]
        public bool hasSuperArmor = false;
        
        [Tooltip("Stamina maliyeti")]
        public float staminaCost = 0f;
    }

    /// <summary>
    /// Kombo dallanması. Hangi input hangi harekete götürür?
    /// </summary>
    [System.Serializable]
    public class ComboBranch
    {
        [Tooltip("Gerekli input tipi")]
        public InputType requiredInput;
        
        [Tooltip("Bu input basılırsa hangi hareket gelir?")]
        public ComboMoveSO nextMove;
        
        [Tooltip("Bu dallanma için özel şart var mı? (örn: sadece havada)")]
        public bool requiresAirborne = false;
        
        [Tooltip("Bu dallanma için minimum combo counter gerekiyor mu?")]
        public int minimumComboCount = 0;
    }
}

