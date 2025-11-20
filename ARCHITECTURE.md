# 🏗️ Neon Syndicate: Retribution - Technical Architecture

Bu doküman, projenin teknik mimarisini ve tasarım kararlarını detaylandırır.

---

## 📐 Mimari Prensipleri

### 1. Composition Over Inheritance
**Neden?** Daha esnek ve yeniden kullanılabilir kod.

```csharp
// ❌ Kalıtım Piramidi (Kötü)
class Character {}
class Player : Character {}
class MeleePlayer : Player {}
class AxelPlayer : MeleePlayer {}

// ✅ Component Kompozisyonu (İyi)
class PlayerController : CharacterBase {}
  + PlayerStateMachine
  + PlayerCombat
  + ComboSystem
```

### 2. Single Responsibility Principle (SRP)
Her sınıfın tek bir sorumluluğu vardır:

- `PlayerController` → Oyuncu durumu ve stat yönetimi
- `PlayerCombat` → Saldırı mekanikleri
- `PlayerStateMachine` → Davranış kontrolü
- `ComboSystem` → Zincir saldırı mantığı

### 3. Interface-Based Design
Polimorfizm için interface'ler:

```csharp
public interface IDamageable {
    void TakeDamage(float damage, Vector2 direction, Transform attacker);
    void Die();
    bool IsAlive();
}
```

Böylece oyuncu, düşman, kırılabilir objeler aynı şekilde hasar alabilir.

---

## 🎮 State Machine Mimarisi

### Finite State Machine (FSM)

**Neden FSM?**
- Karakter davranışlarını net bir şekilde ayırır
- "Spagetti if-else" durumunu önler
- Debug kolaylığı
- State transition kuralları merkezi

### Yapı

```
StateMachineController
  ├── StateBase (Abstract)
  │   ├── Enter()
  │   ├── Update()
  │   ├── FixedUpdate()
  │   └── Exit()
  │
  └── Concrete States
      ├── IdleState
      ├── WalkState
      ├── AttackState
      └── ...
```

### State Geçişleri

```csharp
// PlayerIdleState.cs
public override void Update() {
    if (InputHandler.Instance.MovementInput.magnitude > 0.1f) {
        stateMachine.ChangeState(playerSM.WalkState);
    }
}
```

### State Öncelikleri

```
Death State → En yüksek öncelik (kesintisiz)
  ↓
Hurt State → Orta öncelik (saldırıyı keser)
  ↓
Attack State → Hareketi kısıtlar
  ↓
Walk/Idle State → En düşük öncelik
```

---

## ⚔️ Combat System

### 1. Hitbox/Hurtbox Sistemi

**Hitbox**: Hasar veren bölge (yumruk, tekme)  
**Hurtbox**: Hasar alan bölge (karakter gövdesi)

```
Player Hand (Child GameObject)
  └── Hitbox.cs
      ├── CircleCollider2D (Trigger)
      └── Activate() → Deactivate() döngüsü
```

**Activasyon**: Animator Event'ler ile
```
Punch Animation:
  Frame 3: Call ActivatePunchHitbox()
  Frame 6: Call DeactivateHitboxes()
```

### 2. Combo Sistemi

**Data-Driven Approach**: ScriptableObject kullanımı

```csharp
[CreateAssetMenu]
public class ComboData : ScriptableObject {
    public List<ComboStep> ComboSteps;
}

[Serializable]
public class ComboStep {
    public string AnimationTrigger;
    public float Damage;
    public float CancelWindow;
    public bool IsLauncher;
}
```

**Avantajları**:
- Kodlara dokunmadan combo tasarımı
- A/B test kolaylığı
- Balancing için hızlı iterasyon

### 3. Juggle Mekaniği

```
Launcher Attack (Heavy)
  ↓
Düşman Havaya Kalkar
  ↓
Jump State
  ↓
Air Combo (Light → Light → Heavy)
  ↓
Smash Down (Yere çakma)
```

---

## 🤖 AI Sistemi

### Token-Based Attack System

**Problem**: 10 düşman aynı anda saldırırsa oyun unfair hissettiriyor.

**Çözüm**: Token Sistemi

```
AITokenManager (Singleton)
  ├── maxActiveAttackers = 2
  ├── List<EnemyAI> allEnemies
  └── List<EnemyAI> enemiesWithTokens

Her 0.5 saniyede:
  1. Ölü düşmanları temizle
  2. Tokensiz kalan yerleri doldur
  3. En yakın düşmanlara token ver
```

### Enemy AI States

```
Idle → Devriye veya bekleme
  ↓
Chase → Token yoksa "Circle Strafe", varsa saldırgan takip
  ↓
Attack → Token varsa ve cooldown bittiyse
  ↓
Stunned → Hasar alınca %20 şans ile
```

### Circle Strafe Algoritması

```csharp
Vector2 directionToTarget = (target.position - transform.position).normalized;
Vector2 perpendicular = new Vector2(-directionToTarget.y, directionToTarget.x);
float strafeDirection = Mathf.Sin(Time.time * 2f);
rb.velocity = perpendicular * strafeDirection * moveSpeed * 0.5f;
```

Bu, düşmanların oyuncunun etrafında "tehdit edici" görünmesini sağlar.

---

## 🎯 2.5D Derinlik Sistemi

### Y-Axis Sorting

**Problem**: 2D sprite'lar derinlik algısı veremez.

**Çözüm**: Y pozisyonuna göre Sorting Order ayarla

```csharp
void LateUpdate() {
    int newOrder = Mathf.RoundToInt(-transform.position.y * 100f);
    spriteRenderer.sortingOrder = newOrder;
}
```

**Sonuç**: Ekranda aşağıda (bize yakın) olan karakter önde görünür.

### Jump Implementasyonu

```
Gerçek Fizik KULLANMIYORUZ!
  (Çünkü 2.5D collision karmaşık olur)

Bunun yerine:
  1. Karakterin Y pozisyonu sabit (Collider yerde kalır)
  2. Sprite'ın Y offseti değişir (Görsel yukarı çıkar)
  3. Shadow objesi yerde sabit kalır
```

```csharp
// Parabolic jump
float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
spriteTransform.position = basePosition + Vector3.up * height;
```

---

## 🎨 Ragdoll Physics

### Çalışma Prensibi

**Canlıyken**:
```
Animator.enabled = true
Limb Rigidbody2D.bodyType = Kinematic
```

**Ölünce**:
```
Animator.enabled = false
Limb Rigidbody2D.bodyType = Dynamic
HingeJoint2D → Eklemler aktifleşir
```

### Setup

```
Character (Parent)
  ├── Head
  │   ├── Rigidbody2D (Dynamic when dead)
  │   ├── CircleCollider2D
  │   └── HingeJoint2D (Connected to Body)
  │
  ├── Body
  │   ├── Rigidbody2D
  │   └── BoxCollider2D
  │
  ├── LeftArm
  │   ├── Rigidbody2D
  │   ├── BoxCollider2D
  │   └── HingeJoint2D (Connected to Body)
  │
  └── ... (Diğer uzuvlar)
```

---

## ⚡ Performans Optimizasyonları

### 1. Object Pooling

**Neden Gerekli?**
```
60 FPS oyunda:
  - Her frame 3 kan efekti Instantiate
  - Her frame 2 düşman spawn
  = Saniyede 300 Instantiate çağrısı
  = Garbage Collector patlaması
  = FPS drop
```

**Çözüm**:
```csharp
ObjectPooler.Instance.SpawnFromPool("BloodSplatter", position, rotation);
// Obje kullanıldıktan sonra:
ObjectPooler.Instance.ReturnToPool("BloodSplatter", obj);
```

### 2. String Hashing

```csharp
// ❌ Her frame string comparison (Yavaş)
animator.SetBool("IsWalking", true);

// ✅ Tek seferlik hash (Hızlı)
static readonly int IsWalking = Animator.StringToHash("IsWalking");
animator.SetBool(IsWalking, true);
```

**Performans Farkı**: ~10x daha hızlı

### 3. Component Caching

```csharp
// ❌ Her frame GetComponent çağrısı
void Update() {
    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
}

// ✅ Awake'te cache
private Rigidbody2D rb;
void Awake() {
    rb = GetComponent<Rigidbody2D>();
}
void Update() {
    rb.velocity = Vector2.zero;
}
```

### 4. Update vs FixedUpdate vs LateUpdate

| Metod | Kullanım | Örnek |
|-------|----------|-------|
| `Update()` | Genel oyun mantığı | Input, State geçişleri |
| `FixedUpdate()` | Fizik işlemleri | Rigidbody hareketleri |
| `LateUpdate()` | Kamera, UI güncelleme | Camera follow, Sorting order |

---

## 🔧 Manager Pattern

### Singleton Implementation

```csharp
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

**Avantajları**:
- Global erişim
- Scene geçişlerinde hayatta kalır

**Dezavantajları**:
- Test edilmesi zor
- Tight coupling yaratabilir

**Çözüm**: Event sistemi ile gevşek bağlama

```csharp
// ❌ Tight coupling
GameManager.Instance.AddScore(100);

// ✅ Event-based
public delegate void ScoreEvent(int points);
public static event ScoreEvent OnScoreAdded;
OnScoreAdded?.Invoke(100);
```

---

## 📦 ScriptableObject Kullanımı

### Neden ScriptableObject?

**Problem**: Oyun verileri kodun içinde hardcoded
```csharp
// ❌ Kötü
public class Enemy : MonoBehaviour {
    float health = 50f;
    float damage = 10f;
}
```

**Çözüm**: Data-driven yaklaşım
```csharp
// ✅ İyi
[CreateAssetMenu]
public class EnemyStats : ScriptableObject {
    public float health;
    public float damage;
}

public class Enemy : MonoBehaviour {
    public EnemyStats stats;
    
    void Start() {
        currentHealth = stats.health;
    }
}
```

**Avantajları**:
1. Kodlara dokunmadan balancing
2. A/B testing kolaylığı
3. Aynı prefab, farklı stat'lar
4. Designer-friendly

---

## 🎯 Event Sistemi

### Delegate ve Event Kullanımı

```csharp
// Manager'da
public delegate void GameStateChanged(bool paused);
public static event GameStateChanged OnGamePaused;

// Fırlatma
OnGamePaused?.Invoke(true);

// Dinleme
void OnEnable() {
    GameManager.OnGamePaused += HandlePause;
}

void OnDisable() {
    GameManager.OnGamePaused -= HandlePause;
}

void HandlePause(bool isPaused) {
    // Logic burada
}
```

**Avantajları**:
- Loose coupling
- Modüler kod
- Observer pattern

---

## 🔍 Debug Stratejileri

### 1. Gizmo Kullanımı

```csharp
private void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
```

### 2. Inspector'da State Gösterme

```csharp
[SerializeField] private string currentStateName; // Debug için

void Update() {
    if (showDebugInfo) {
        currentStateName = CurrentState?.GetType().Name;
    }
}
```

### 3. Context Menu

```csharp
[ContextMenu("Test Attack")]
void TestAttack() {
    stateMachine.ChangeState(attackState);
}
```

---

## 📊 Gelecek İyileştirmeler

### 1. Ability System
Mevcut yapı "sert kodlanmış" kombolar kullanıyor. Gelecekte:
```
ScriptableObject-based Ability System
  - Her ability ayrı ScriptableObject
  - Runtime'da yeni ability ekleme
  - Ability şartları (stamina, cooldown)
```

### 2. Behavior Tree AI
Token sistemi iyi çalışıyor ama karmaşık davranışlar için:
```
Behavior Tree
  ├── Selector
  │   ├── Sequence (Attack)
  │   └── Sequence (Patrol)
  └── Condition (Health < 30%)
```

### 3. Animation Graph (Blend Tree)
Mevcut: Discrete state'ler  
Gelecek: Smooth geçişler

```
Blend Tree
  ├── Idle (0-0.1 speed)
  ├── Walk (0.1-0.5 speed)
  └── Run (0.5-1 speed)
```

---

## 🎓 Kaynaklar ve Referanslar

### Öğrenme Materyalleri
- [Unity State Machine Tutorial](https://unity.com)
- [Object Pooling Best Practices](https://learn.unity.com/tutorial/object-pooling)
- [ScriptableObject Architecture](https://www.youtube.com/watch?v=raQ3iHhE_Kk)

### İlham Kaynakları
- **Crazy Flasher** - Fizik ve dövüş hissi
- **Dead Cells** - Smooth combat
- **Streets of Rage 4** - Modern beat 'em up standartı

---

**Bu mimari, sürekli evrim geçirecek. Feedback ve önerileriniz değerlidir!** 🚀

