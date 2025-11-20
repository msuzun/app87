using UnityEngine;
using UnityEngine.Events;

namespace NeonSyndicate.Animation
{
    /// <summary>
    /// Animation Event Receiver - Unity Animation Event'lerini dinler.
    /// Animator'a doğrudan bağlı objeye eklenir (genelde sprite objesi).
    /// 
    /// Kullanım:
    /// 1. Unity Animation window'da event ekle
    /// 2. Function name: "AE_EnableHitbox" (örnek)
    /// 3. Bu script otomatik çağrılır
    /// 4. C# event olarak yukarı fırlatılır
    /// 5. Combat script dinler ve hitbox'ı açar
    /// 
    /// = FRAME-PERFECT COMBAT!
    /// </summary>
    public class AnimationEventReceiver : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool logEvents = false;

        #region Combat Events
        /// <summary>
        /// Hitbox aktifleştirilmeli (vuruş karesinde).
        /// </summary>
        public event UnityAction OnHitboxEnable;
        
        /// <summary>
        /// Hitbox deaktif edilmeli.
        /// </summary>
        public event UnityAction OnHitboxDisable;
        
        /// <summary>
        /// Combo window açılır. Oyuncu bu andan itibaren kombo yapabilir.
        /// </summary>
        public event UnityAction OnComboWindowOpen;
        
        /// <summary>
        /// Combo window kapanır.
        /// </summary>
        public event UnityAction OnComboWindowClose;
        
        /// <summary>
        /// Animasyon tamamlandı.
        /// </summary>
        public event UnityAction OnAnimationComplete;
        #endregion

        #region Movement Events
        /// <summary>
        /// Ayak sesi event'i.
        /// </summary>
        public event UnityAction OnFootstep;
        
        /// <summary>
        /// Zıplama başladı.
        /// </summary>
        public event UnityAction OnJumpStart;
        
        /// <summary>
        /// Yere indi.
        /// </summary>
        public event UnityAction OnLand;
        #endregion

        #region VFX Events
        /// <summary>
        /// VFX spawn et. String parameter: effect name.
        /// </summary>
        public event UnityAction<string> OnSpawnVFX;
        
        /// <summary>
        /// Trail effect başlat.
        /// </summary>
        public event UnityAction OnTrailStart;
        
        /// <summary>
        /// Trail effect durdur.
        /// </summary>
        public event UnityAction OnTrailStop;
        #endregion

        #region Camera Events
        /// <summary>
        /// Kamera sarsma. Float parameter: intensity.
        /// </summary>
        public event UnityAction<float> OnCameraShake;
        #endregion

        #region Invulnerability Events
        /// <summary>
        /// I-Frame başlar (dodge, roll vb.).
        /// </summary>
        public event UnityAction OnIFrameStart;
        
        /// <summary>
        /// I-Frame biter.
        /// </summary>
        public event UnityAction OnIFrameEnd;
        #endregion

        // ========================================
        // UNITY ANIMATION EVENT CALLBACKS
        // (Bu metotlar Animation window'dan çağrılır)
        // ========================================

        #region Combat Callbacks
        /// <summary>
        /// Animation Event: Hitbox'ı aktifleştir.
        /// Animasyonun vuruş karesine eklenir.
        /// </summary>
        public void AE_EnableHitbox()
        {
            OnHitboxEnable?.Invoke();
            Log("Hitbox ENABLED");
        }

        /// <summary>
        /// Animation Event: Hitbox'ı deaktif et.
        /// Vuruş karesinden sonra eklenir.
        /// </summary>
        public void AE_DisableHitbox()
        {
            OnHitboxDisable?.Invoke();
            Log("Hitbox DISABLED");
        }

        /// <summary>
        /// Animation Event: Combo window aç.
        /// </summary>
        public void AE_OpenComboWindow()
        {
            OnComboWindowOpen?.Invoke();
            Log("Combo Window OPEN");
        }

        /// <summary>
        /// Animation Event: Combo window kapat.
        /// </summary>
        public void AE_CloseComboWindow()
        {
            OnComboWindowClose?.Invoke();
            Log("Combo Window CLOSED");
        }

        /// <summary>
        /// Animation Event: Animasyon tamamlandı.
        /// Genelde animasyonun son karesine eklenir.
        /// </summary>
        public void AE_AnimationFinish()
        {
            OnAnimationComplete?.Invoke();
            Log("Animation COMPLETE");
        }
        #endregion

        #region Movement Callbacks
        /// <summary>
        /// Animation Event: Ayak sesi.
        /// Walk/Run animasyonunda ayak yere değdiği karelerde çağrılır.
        /// </summary>
        public void AE_Footstep()
        {
            OnFootstep?.Invoke();
            Log("FOOTSTEP");
        }

        /// <summary>
        /// Animation Event: Zıplama başladı.
        /// </summary>
        public void AE_JumpStart()
        {
            OnJumpStart?.Invoke();
            Log("JUMP START");
        }

        /// <summary>
        /// Animation Event: Yere indi.
        /// </summary>
        public void AE_Land()
        {
            OnLand?.Invoke();
            Log("LAND");
        }
        #endregion

        #region VFX Callbacks
        /// <summary>
        /// Animation Event: VFX spawn.
        /// String parameter ile effect ismi gönderilir.
        /// 
        /// Kullanım: Animation Event > Function: AE_SpawnVFX > String: "DustCloud"
        /// </summary>
        public void AE_SpawnVFX(string effectName)
        {
            OnSpawnVFX?.Invoke(effectName);
            Log($"SPAWN VFX: {effectName}");
        }

        /// <summary>
        /// Animation Event: Trail başlat.
        /// </summary>
        public void AE_TrailStart()
        {
            OnTrailStart?.Invoke();
            Log("TRAIL START");
        }

        /// <summary>
        /// Animation Event: Trail durdur.
        /// </summary>
        public void AE_TrailStop()
        {
            OnTrailStop?.Invoke();
            Log("TRAIL STOP");
        }
        #endregion

        #region Camera Callbacks
        /// <summary>
        /// Animation Event: Kamera sarsma.
        /// Float parameter: intensity (0.1 = hafif, 1.0 = güçlü)
        /// 
        /// Kullanım: Animation Event > Function: AE_CameraShake > Float: 0.5
        /// </summary>
        public void AE_CameraShake(float intensity)
        {
            OnCameraShake?.Invoke(intensity);
            Log($"CAMERA SHAKE: {intensity}");
        }

        /// <summary>
        /// Overload: Parametresiz versiyon (default intensity).
        /// </summary>
        public void AE_CameraShake()
        {
            AE_CameraShake(0.3f);
        }
        #endregion

        #region Invulnerability Callbacks
        /// <summary>
        /// Animation Event: I-Frame başlat.
        /// Dodge animasyonunun başında çağrılır.
        /// </summary>
        public void AE_IFrameStart()
        {
            OnIFrameStart?.Invoke();
            Log("I-FRAME START");
        }

        /// <summary>
        /// Animation Event: I-Frame bitir.
        /// Dodge animasyonunun sonunda çağrılır.
        /// </summary>
        public void AE_IFrameEnd()
        {
            OnIFrameEnd?.Invoke();
            Log("I-FRAME END");
        }
        #endregion

        #region Utility
        private void Log(string message)
        {
            if (logEvents)
            {
                Debug.Log($"[AnimEvent] {gameObject.name}: {message}");
            }
        }

        /// <summary>
        /// Tüm event subscription'ları temizler.
        /// Memory leak önlemek için OnDestroy'da çağrılabilir.
        /// </summary>
        public void ClearAllEvents()
        {
            OnHitboxEnable = null;
            OnHitboxDisable = null;
            OnComboWindowOpen = null;
            OnComboWindowClose = null;
            OnAnimationComplete = null;
            OnFootstep = null;
            OnJumpStart = null;
            OnLand = null;
            OnSpawnVFX = null;
            OnTrailStart = null;
            OnTrailStop = null;
            OnCameraShake = null;
            OnIFrameStart = null;
            OnIFrameEnd = null;
        }
        #endregion
    }
}

