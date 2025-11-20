using UnityEngine;

namespace NeonSyndicate.Audio
{
    /// <summary>
    /// Audio Event Helper - Animation Event'lerden ses çalmak için.
    /// AnimationEventReceiver ile entegre çalışır.
    /// 
    /// Kullanım:
    /// 1. Character'e bu component'i ekle
    /// 2. AnimationEventReceiver event'lerine subscribe et
    /// 3. Animation timeline'dan otomatik ses çalınır
    /// </summary>
    public class AudioEventHelper : MonoBehaviour
    {
        [Header("Attack Sounds")]
        [SerializeField] private string[] punchWhooshSounds = { "Whoosh_Light", "Whoosh_Medium" };
        [SerializeField] private string[] kickWhooshSounds = { "Whoosh_Heavy", "Whoosh_Kick" };
        [SerializeField] private string[] impactSounds = { "Impact_Flesh", "Impact_Bone" };

        [Header("Movement Sounds")]
        [SerializeField] private string[] footstepSounds = { "Step_Concrete", "Step_Concrete_2" };
        [SerializeField] private string jumpSound = "Jump_Whoosh";
        [SerializeField] private string landSound = "Land_Impact";
        [SerializeField] private string dashSound = "Dash_Whoosh";

        [Header("Damage Sounds")]
        [SerializeField] private string[] hurtSounds = { "Player_Hurt_1", "Player_Hurt_2" };
        [SerializeField] private string deathSound = "Player_Death";

        private Animation.AnimationEventReceiver eventReceiver;

        private void Start()
        {
            eventReceiver = GetComponentInChildren<Animation.AnimationEventReceiver>();
            
            if (eventReceiver != null)
            {
                SubscribeToEvents();
            }
        }

        private void OnDestroy()
        {
            if (eventReceiver != null)
            {
                UnsubscribeFromEvents();
            }
        }

        #region Event Subscription
        private void SubscribeToEvents()
        {
            eventReceiver.OnFootstep += PlayFootstepSound;
            eventReceiver.OnJumpStart += PlayJumpSound;
            eventReceiver.OnLand += PlayLandSound;
            // Punch/kick ses hale animation'dan direct çağrılacak (AE_PlayPunchSound vb.)
        }

        private void UnsubscribeFromEvents()
        {
            eventReceiver.OnFootstep -= PlayFootstepSound;
            eventReceiver.OnJumpStart -= PlayJumpSound;
            eventReceiver.OnLand -= PlayLandSound;
        }
        #endregion

        #region Sound Playback
        /// <summary>
        /// Adım sesi (animation event).
        /// </summary>
        private void PlayFootstepSound()
        {
            string footstep = footstepSounds[Random.Range(0, footstepSounds.Length)];
            ProAudioManager.Instance?.PlaySFX(footstep);
        }

        /// <summary>
        /// Zıplama sesi.
        /// </summary>
        private void PlayJumpSound()
        {
            ProAudioManager.Instance?.PlaySFX(jumpSound);
        }

        /// <summary>
        /// Yere iniş sesi.
        /// </summary>
        private void PlayLandSound()
        {
            ProAudioManager.Instance?.PlaySFX(landSound);
        }

        // === Animation Event'lerden direkt çağrılacak metodlar ===
        // (AE_ prefix ile AnimationEventReceiver'a ek olarak)

        /// <summary>
        /// Animation Event: Hafif yumruk whoosh.
        /// </summary>
        public void AE_PlayPunchWhoosh()
        {
            string whoosh = punchWhooshSounds[Random.Range(0, punchWhooshSounds.Length)];
            ProAudioManager.Instance?.PlaySFX(whoosh);
        }

        /// <summary>
        /// Animation Event: Ağır tekme whoosh.
        /// </summary>
        public void AE_PlayKickWhoosh()
        {
            string whoosh = kickWhooshSounds[Random.Range(0, kickWhooshSounds.Length)];
            ProAudioManager.Instance?.PlaySFX(whoosh);
        }

        /// <summary>
        /// Animation Event: Vuruş impact sesi (Hitbox'tan da çağrılır).
        /// </summary>
        public void AE_PlayImpactSound()
        {
            string impact = impactSounds[Random.Range(0, impactSounds.Length)];
            ProAudioManager.Instance?.PlaySFX(impact);
        }

        /// <summary>
        /// Animation Event: Layered heavy punch (whoosh + impact + bone).
        /// Crazy Flasher tarzı katmanlı ses!
        /// </summary>
        public void AE_PlayHeavyPunchLayered()
        {
            ProAudioManager.Instance?.PlayLayeredSound(
                "Whoosh_Heavy",
                "Impact_Flesh",
                "Impact_Bone"
            );
        }

        /// <summary>
        /// Animation Event: Dash sesi.
        /// </summary>
        public void AE_PlayDashSound()
        {
            ProAudioManager.Instance?.PlaySFX(dashSound);
        }

        /// <summary>
        /// Animation Event: Hurt ses (oyuncu hasar alınca).
        /// </summary>
        public void AE_PlayHurtSound()
        {
            string hurt = hurtSounds[Random.Range(0, hurtSounds.Length)];
            ProAudioManager.Instance?.PlaySFX(hurt);
        }

        /// <summary>
        /// Animation Event: Ölüm sesi.
        /// </summary>
        public void AE_PlayDeathSound()
        {
            ProAudioManager.Instance?.PlaySFX(deathSound);
        }
        #endregion
    }
}

