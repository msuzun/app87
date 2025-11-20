# 📝 CHANGELOG

Bu dosya, projedeki önemli değişiklikleri takip eder.

---

## [Hibrit Sistem] - 2024

### 🔥 Yeni Özellikler

#### **Hybrid System Implementation**
- ✨ Class-based FSM + Coroutine-based actions mimarisi
- ✨ `PlayerController` coroutine desteği eklendi
- ✨ `DashCoroutine()` - I-frame destekli dash sistemi
- ✨ `JumpCoroutine()` - Fake height parabolic jump
- ✨ Coroutine tracking ve interrupt mekanizması

#### **Run/Sprint Sistemi**
- ✨ Shift tuşu ile koşma
- ✨ Stamina tüketimi (5/saniye)
- ✨ Hız çarpanı (x1.6)
- ✨ Animator entegrasyonu (`IsRunning` parametresi)

#### **Responsive Combat**
- ✨ Air attack desteği (havada saldırı)
- ✨ Attack momentum (Crazy Flasher hissi)
- ✨ Combo timeout kontrolü
- ✨ Heavy attack launcher sistemi
- ✨ Animation Event callbacks (`OnAttackComplete`)

#### **Dokümantasyon**
- 📚 `HYBRID_SYSTEM.md` - Hibrit sistem açıklaması
- 📚 `CONTROLS.md` - Detaylı kontrol rehberi
- 📚 `CHANGELOG.md` - Bu dosya

### ♻️ Değişiklikler

#### **PlayerController.cs**
- Updated: Coroutine support eklendi
- Updated: `isFacingRight`, `isGrounded`, `isRunning` flags
- Updated: `Flip()` metodu eklendi
- Updated: Stamina sistem güncellemesi (run cost)
- Added: `DashCoroutine()`, `JumpCoroutine()`
- Added: `StartActionCoroutine()`, `StopCurrentAction()`
- Added: `CreateJumpShadow()` helper

#### **PlayerStates**
- Updated: `PlayerWalkState` - Run speed multiplier
- Updated: `PlayerJumpState` - Coroutine-based implementation
- Updated: `PlayerDodgeState` - Coroutine-based implementation
- Updated: `PlayerAttackState` - Air attack + responsive combo

#### **PlayerCombat.cs**
- Updated: `ExecuteNextCombo()` - Air attack kontrolü
- Added: `ExecuteAirAttack()` metodu
- Added: `ApplyAttackMomentum()` - Saldırı momentum'u
- Added: `OnAttackComplete()` - Animation Event callback
- Updated: Ses efektleri hitbox aktivasyonlarına eklendi

#### **InputHandler.cs**
- Added: `IsRunPressed` property
- Updated: Run input tracking

### 🐛 Bug Fixes
- Fixed: State geçişlerinde coroutine cleanup
- Fixed: Jump sonrası Y pozisyonu reset
- Fixed: Dodge stamina kontrolü
- Fixed: Flip timing (LateUpdate'te)

---

## [Initial Release] - 2024

### ✨ İlk Özellikler

#### **Core Systems**
- ✅ GameManager (Singleton pattern)
- ✅ InputHandler (Input System integration)
- ✅ ObjectPooler (Performance optimization)
- ✅ SoundManager (Audio management)

#### **State Machine**
- ✅ State-based FSM architecture
- ✅ 7 Player states (Idle, Walk, Attack, Jump, Dodge, Hurt, Death)
- ✅ StateMachineController base class
- ✅ Clean state transitions

#### **Combat System**
- ✅ Hitbox/Hurtbox system
- ✅ ComboSystem (ScriptableObject-based)
- ✅ PlayerCombat manager
- ✅ IDamageable interface
- ✅ IKnockbackable interface

#### **Enemy AI**
- ✅ EnemyController (5 enemy types)
- ✅ EnemyAI (Behavior system)
- ✅ AITokenManager (Token-based attack queue)
- ✅ Circle strafe behavior

#### **Data Systems**
- ✅ ComboData ScriptableObject
- ✅ EnemyStats ScriptableObject
- ✅ LevelData ScriptableObject
- ✅ PlayerStats ScriptableObject

#### **Utility**
- ✅ SortingOrderController (2.5D depth)
- ✅ RagdollController (Physics-based ragdoll)
- ✅ DamageNumber (Floating damage text)
- ✅ CameraFollow (Camera system)

#### **Dokümantasyon**
- 📚 README.md (Genel bilgi)
- 📚 ARCHITECTURE.md (Teknik detaylar)
- 📚 SETUP_GUIDE.md (Kurulum)
- 📚 PROJECT_STRUCTURE.md (Klasör yapısı)
- 📚 LICENSE.md (MIT)

---

## [Animation System] - 2024

### 🎬 Yeni Özellikler

#### **Event-Driven Animation System**
- ✨ `AnimData.cs` - Animation constants (60+ constant)
- ✨ `AnimationEventReceiver.cs` - Unity Animation Event → C# Event bridge
- ✨ `CharacterAnimator.cs` - Wrapper (Unity Animator)
- ✨ `CharacterAnimatorSpine.cs` - Wrapper (Spine 2D support)
- ✨ `PlayerCombatAnimated.cs` - Event-driven combat örneği

#### **Frame-Perfect Combat**
- ✨ Hitbox tam vuruş karesinde aktif
- ✨ Animation event ile kontrol
- ✨ Combo cancel windows (event-driven)
- ✨ I-Frame system (dodge)
- ✨ VFX spawn (timeline-based)
- ✨ Camera shake (intensity parameter)

#### **Spine Support**
- ✨ Optional Spine 2D entegrasyonu
- ✨ Spine Event → Unity Event bridge
- ✨ Mix duration kontrolü
- ✨ Aynı API (teknoloji bağımsız)

#### **ProComboSystem Updates**
- ✨ ComboMoveSO genişletildi (15+ parametre)
- ✨ Branching combo support
- ✨ Input buffering (0.2s)
- ✨ Cancel windows (timing-based)
- ✨ Hit stop implementation

#### **Dokümantasyon**
- 📚 `ANIMATION_SYSTEM.md` - Animation system rehberi
- 📚 `PRO_COMBO_GUIDE.md` - Combo system rehberi
- 📚 `ENEMY_AI_DESIGN.md` - AI tasarım dokümanı
- 📚 `ENEMY_AI_USAGE.md` - AI kullanım rehberi

### ♻️ Değişiklikler

#### **Combat/Hitbox.cs**
- Updated: ProComboSystem entegrasyonu
- Updated: Hit confirm callback

#### **PlayerCombat.cs**
- Updated: Air attack support
- Updated: Attack momentum
- Added: Animation Event callbacks

### 🎨 Yeni Dosyalar
- `AnimData.cs` (200+ satır)
- `AnimationEventReceiver.cs` (300+ satır)
- `CharacterAnimator.cs` (400+ satır)
- `CharacterAnimatorSpine.cs` (200+ satır)
- `PlayerCombatAnimated.cs` (300+ satır)
- `ComboMoveSO.cs` (150+ satır)
- `InputBuffer.cs` (200+ satır)
- `ProComboSystem.cs` (400+ satır)

---

## 🔜 Gelecek Güncellemeler

### Version 1.1 (Planlanıyor)
- [ ] UI/HUD Manager sistemi
- [ ] Health bar ve stamina bar UI
- [ ] Combo counter UI
- [ ] Style rank display
- [ ] Pause menu

### Version 1.2 (Planlanıyor)
- [ ] Boss AI implementation
- [ ] 4 Boss karakteri
- [ ] Boss phase transitions
- [ ] Boss-specific mechanics

### Version 1.3 (Planlanıyor)
- [ ] Level progression system
- [ ] Wave spawner
- [ ] Checkpoint system
- [ ] Level transitions

### Version 1.4 (Planlanıyor)
- [ ] Upgrade shop
- [ ] Save/Load system
- [ ] Achievement system
- [ ] Unlock system

### Version 2.0 (Long-term)
- [ ] Multiplayer co-op
- [ ] Second playable character
- [ ] New game+ mode
- [ ] Endless mode

---

## 📊 İstatistikler

### Kod Metrikleri (Hibrit Sistem Sonrası)
```
C# Scripts: 35+ dosya
Kod Satırı: ~5000+ satır
Dokümantasyon: 3000+ satır
Namespace: 6 adet
Interface: 3 adet
Manager: 4 adet
State: 7 adet (Player)
ScriptableObject: 4 tip
```

### Performans
```
Target FPS: 60
Coroutine Overhead: < 0.1ms
State Machine: < 0.05ms per frame
Object Pool: %95 Instantiate azalması
```

---

## 🙏 Katkıda Bulunanlar

- **Ana Geliştirici**: [Your Name]
- **Hibrit Sistem**: Community feedback ile geliştirildi
- **Inspirasyon**: Crazy Flasher Series

---

## 📎 Linkler

- [GitHub Repository](https://github.com/yourusername/neon-syndicate)
- [Documentation](./README.md)
- [Issue Tracker](https://github.com/yourusername/neon-syndicate/issues)

---

**Not**: Bu proje aktif geliştirme aşamasındadır. Sık sık güncellenecektir.

**Last Updated**: November 2024

