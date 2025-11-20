using UnityEngine;

namespace NeonSyndicate.Animation
{
    /// <summary>
    /// Animation Constants - String hatalarını önlemek için.
    /// Tüm animasyon isimleri ve event isimleri burada tanımlanır.
    /// 
    /// Kullanım:
    /// animator.Play(AnimData.ATTACK_1); // ✓ Güvenli
    /// animator.Play("Attack1"); // ✗ Typo riski!
    /// </summary>
    public static class AnimData
    {
        #region Animator State Names
        // === MOVEMENT STATES ===
        public const string IDLE = "Idle";
        public const string WALK = "Walk";
        public const string RUN = "Run";
        public const string SPRINT = "Sprint";
        
        // === JUMP & AIR ===
        public const string JUMP = "Jump";
        public const string FALL = "Fall";
        public const string LAND = "Land";
        
        // === COMBAT - LIGHT ATTACKS ===
        public const string ATTACK_LIGHT_1 = "Attack_Light_1";
        public const string ATTACK_LIGHT_2 = "Attack_Light_2";
        public const string ATTACK_LIGHT_3 = "Attack_Light_3";
        
        // === COMBAT - HEAVY ATTACKS ===
        public const string ATTACK_HEAVY = "Attack_Heavy";
        public const string ATTACK_HEAVY_LAUNCHER = "Attack_Heavy_Launcher";
        
        // === COMBAT - SPECIAL ===
        public const string ATTACK_AIR = "Attack_Air";
        public const string ATTACK_GRAB = "Grab_Attempt";
        public const string ATTACK_THROW = "Throw";
        
        // === DEFENSE ===
        public const string DODGE = "Dodge";
        public const string DASH = "Dash";
        public const string BLOCK = "Block";
        
        // === DAMAGE & DEATH ===
        public const string HIT = "Hit";
        public const string HURT = "Hurt";
        public const string STUNNED = "Stunned";
        public const string KNOCKOUT = "Knockout";
        public const string DEATH = "Death";
        
        // === EXECUTION (RAGE MOVE) ===
        public const string EXECUTION = "Execution_Move";
        #endregion

        #region Animator Parameters
        // === BOOL PARAMETERS ===
        public const string PARAM_IS_WALKING = "IsWalking";
        public const string PARAM_IS_RUNNING = "IsRunning";
        public const string PARAM_IS_GROUNDED = "IsGrounded";
        public const string PARAM_IS_ATTACKING = "IsAttacking";
        public const string PARAM_IS_DEAD = "IsDead";
        
        // === FLOAT PARAMETERS ===
        public const string PARAM_SPEED = "Speed";
        public const string PARAM_VERTICAL_VELOCITY = "VerticalVelocity";
        
        // === TRIGGER PARAMETERS ===
        public const string PARAM_ATTACK = "Attack";
        public const string PARAM_HIT_TRIGGER = "Hit";
        public const string PARAM_DODGE_TRIGGER = "Dodge";
        public const string PARAM_DEATH_TRIGGER = "Die";
        #endregion

        #region Animation Event Names
        // === COMBAT EVENTS ===
        /// <summary>
        /// Hitbox'ı aktifleştirir. Animasyonun vuruş karesinde çağrılmalı.
        /// </summary>
        public const string EVT_HITBOX_ENABLE = "AE_EnableHitbox";
        
        /// <summary>
        /// Hitbox'ı deaktif eder. Vuruş karesinden sonra çağrılmalı.
        /// </summary>
        public const string EVT_HITBOX_DISABLE = "AE_DisableHitbox";
        
        /// <summary>
        /// Combo window'u açar. Bu andan itibaren oyuncu kombo yapabilir.
        /// </summary>
        public const string EVT_COMBO_WINDOW_OPEN = "AE_OpenComboWindow";
        
        /// <summary>
        /// Combo window'u kapatır. Artık kombo yapılamaz.
        /// </summary>
        public const string EVT_COMBO_WINDOW_CLOSE = "AE_CloseComboWindow";
        
        /// <summary>
        /// Animasyon tamamlandı sinyali. State değiştirmek için kullanılabilir.
        /// </summary>
        public const string EVT_ANIMATION_FINISH = "AE_AnimationFinish";
        
        // === MOVEMENT EVENTS ===
        /// <summary>
        /// Ayak yere değdiğinde çağrılır. Ses efekti için.
        /// </summary>
        public const string EVT_FOOTSTEP = "AE_Footstep";
        
        /// <summary>
        /// Zıplama anında çağrılır.
        /// </summary>
        public const string EVT_JUMP_START = "AE_JumpStart";
        
        /// <summary>
        /// Yere indiğinde çağrılır.
        /// </summary>
        public const string EVT_LAND = "AE_Land";
        
        // === VFX EVENTS ===
        /// <summary>
        /// Particle effect spawn et.
        /// </summary>
        public const string EVT_SPAWN_VFX = "AE_SpawnVFX";
        
        /// <summary>
        /// Trail effect başlat (dash vb. için).
        /// </summary>
        public const string EVT_TRAIL_START = "AE_TrailStart";
        
        /// <summary>
        /// Trail effect durdur.
        /// </summary>
        public const string EVT_TRAIL_STOP = "AE_TrailStop";
        
        // === CAMERA EVENTS ===
        /// <summary>
        /// Kamera sarsma efekti.
        /// </summary>
        public const string EVT_CAMERA_SHAKE = "AE_CameraShake";
        
        // === INVULNERABILITY ===
        /// <summary>
        /// I-Frame başlat (dodge sırasında).
        /// </summary>
        public const string EVT_IFRAME_START = "AE_IFrameStart";
        
        /// <summary>
        /// I-Frame bitir.
        /// </summary>
        public const string EVT_IFRAME_END = "AE_IFrameEnd";
        #endregion

        #region Helper Methods
        /// <summary>
        /// Light attack serisi için state ismini döner.
        /// </summary>
        public static string GetLightAttack(int index)
        {
            return index switch
            {
                1 => ATTACK_LIGHT_1,
                2 => ATTACK_LIGHT_2,
                3 => ATTACK_LIGHT_3,
                _ => ATTACK_LIGHT_1
            };
        }

        /// <summary>
        /// Animator parametre hash'lerini cache etmek için.
        /// String yerine int hash kullanmak performanslıdır.
        /// </summary>
        public static class Hash
        {
            public static readonly int IsWalking = Animator.StringToHash(PARAM_IS_WALKING);
            public static readonly int IsRunning = Animator.StringToHash(PARAM_IS_RUNNING);
            public static readonly int IsGrounded = Animator.StringToHash(PARAM_IS_GROUNDED);
            public static readonly int IsAttacking = Animator.StringToHash(PARAM_IS_ATTACKING);
            public static readonly int IsDead = Animator.StringToHash(PARAM_IS_DEAD);
            
            public static readonly int Speed = Animator.StringToHash(PARAM_SPEED);
            public static readonly int VerticalVelocity = Animator.StringToHash(PARAM_VERTICAL_VELOCITY);
            
            public static readonly int Attack = Animator.StringToHash(PARAM_ATTACK);
            public static readonly int Hit = Animator.StringToHash(PARAM_HIT_TRIGGER);
            public static readonly int Dodge = Animator.StringToHash(PARAM_DODGE_TRIGGER);
            public static readonly int Die = Animator.StringToHash(PARAM_DEATH_TRIGGER);
        }
        #endregion
    }
}

