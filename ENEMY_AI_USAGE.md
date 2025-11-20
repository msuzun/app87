# 🎮 ENEMY AI KULLANIM REHBERİ

3 farklı düşman AI'sını Unity'de nasıl kullanacağınızı anlatan pratik rehber.

---

## 📦 Kurulum

### 1. Prefab Hazırlama

#### Basic Brawler Prefab
```
Hierarchy:
  - Enemy_BasicBrawler
    ├── Sprite (Body)
    ├── AttackPoint (Empty GameObject)
    │   └── Hitbox (CircleCollider2D + Hitbox.cs)
    └── Hurtbox (BoxCollider2D + Hurtbox.cs)

Components:
  - Rigidbody2D (Gravity Scale: 0, Freeze Rotation Z)
  - BoxCollider2D
  - Animator
  - SpriteRenderer
  - SortingOrderController
  - EnemyController (Type: Thug)
  - BasicBrawlerAI ← YENİ!
```

#### Fast Dodger Prefab
```
Enemy_FastDodger
  (Aynı yapı, ama BasicBrawlerAI yerine)
  - FastDodgerAI ← YENİ!
  
Stats farklı:
  HP: 40 (daha düşük)
  Speed: 6.0 (2x daha hızlı)
```

#### Heavy Tank Prefab
```
Enemy_HeavyTank
  (Daha büyük sprite)
  - HeavyTankAI ← YENİ!
  
Stats farklı:
  HP: 150 (çok yüksek)
  Speed: 2.0 (yavaş)
  Scale: 1.5x (daha büyük)
```

---

### 2. Animator Controller Setup

Her AI tipi için **Animation State** gerekli:

#### Basic Brawler Animator
```
Parameters:
  - IsWalking (Bool)
  - Attack (Trigger)
  - Hurt (Trigger)
  - Die (Trigger)

States:
  - Idle
  - Walk
  - Attack_Light
  - Attack_Heavy
  - Grab_Attempt
  - Hurt
  - Death
```

#### Fast Dodger Animator
```
Extra States:
  - Dash_In
  - Dash_Out
  - Quick_Stab
  - Dodge_Roll
  - Backflip
```

#### Heavy Tank Animator
```
Extra States:
  - Heavy_Windup
  - Heavy_Swing
  - Charge_Roar
  - Charging
  - Ground_Slam
  - Grab_Attempt
  - Grab_Success
  - Throw
  - Stunned (önemli!)
```

---

### 3. Animation Events

**Her saldırı animasyonuna event ekle:**

```csharp
// Frame 3'te (vuruş anı):
Function: ActivateAttackHitbox
Time: 0.15 (örnek)

// Animasyon sonunda:
Function: OnAttackComplete (opsiyonel)
```

---

## 🎯 Inspector Ayarları

### Basic Brawler
```
Basic AI Settings:
  Detection Range: 8
  Attack Range: 1.5
  Attack Cooldown: 2.0

AI Personality:
  Aggressiveness: 0.5
  Caution: 0.3
  Intelligence: 0.7

Brawler Specific:
  Patrol Radius: 5
  Back Off Distance: 2
  Combo Chance: 0.7
```

### Fast Dodger
```
Basic AI Settings:
  Detection Range: 10 (daha uzun görüş)
  Attack Range: 1.2
  Attack Cooldown: 1.5 (daha hızlı)

Dodger Specific:
  Preferred Distance: 6 (kite mesafesi)
  Dodge Chance: 0.4 (% 40)
  Dash Speed: 10
  Observe Time: 2
```

### Heavy Tank
```
Basic AI Settings:
  Detection Range: 6 (daha kısa)
  Attack Range: 2.0
  Attack Cooldown: 3.0 (daha yavaş)

Tank Specific:
  Charge Speed: 6
  Charge Range: 8
  Grab Range: 1.5
  Ground Pound Range: 3
  Has Super Armor: ✓ (true)
  Berserker Threshold: 0.3 (HP %30)
```

---

## 🎮 Sahne Kurulumu

### Test Sahnesi

```
Hierarchy:
  - GameManager
    + AITokenManager ← ÖNEMLİ!
  
  - Player (0, 0, 0)
  
  - Enemies
    ├── BasicBrawler_1 (3, 0, 0)
    ├── BasicBrawler_2 (5, 1, 0)
    ├── FastDodger_1 (7, 0, 0)
    └── HeavyTank_1 (10, 0, 0)
  
  - Environment
    └── Walls (Tag: "Wall" <- Heavy Tank için önemli!)
```

---

## 🔧 Davranış Testi

### Basic Brawler Test
```
1. Play mode'a gir
2. Brawler'a yaklaş
   ✓ Detection range'de chase başlamalı
3. Token alması için bekle
   ✓ Attack range'de saldırmalı
4. Saldırı combo yapmalı (Jab -> Cross)
5. Token yoksa:
   ✓ Circle strafe yapmalı
   ✓ Çok yakınsa back off etmeli
```

### Fast Dodger Test
```
1. Dodger'a yaklaş
   ✓ Kaçmalı (preferred distance korur)
2. Token aldığında:
   ✓ Hit-and-run yapmalı (Dash in -> Strike -> Dash out)
3. Saldırı yapmayı dene:
   ✓ %40 şans ile dodge roll yapmalı
4. Köşeye sıkıştır:
   ✓ Desperate behavior (more aggressive)
```

### Heavy Tank Test
```
1. Tank'e yaklaş
   ✓ Yavaşça intimidate walk
2. Token aldığında:
   ✓ Yakınsa: Grab attempt
   ✓ Orta mesafe: Heavy swing
   ✓ Uzakta: Charge attack
3. Charge'ı duvara çarptır:
   ✓ 2 saniye stunned olmalı (vulnerable!)
4. HP'yi %30'un altına düşür:
   ✓ Berserker mode aktif olmalı
   ✓ Daha hızlı ve agresif
```

---

## 🎨 Visual Debugging

### Console Logs

Her AI debug log yazdırır:

```csharp
// BasicBrawlerAI
"Grab Success!"
"Brawler changed state: Chase -> Attack"

// FastDodgerAI
"Dodge Roll activated!"
"Hit-and-Run combo complete"

// HeavyTankAI
"Heavy Tank entered BERSERKER MODE!"
"Charge hit wall - Stunned!"
```

### Gizmos

Scene view'da görülebilir:

```
Sarı Çember: Detection Range
Kırmızı Çember: Attack Range
Mavi Çember: Charge Range (Tank)
Mor Çember: Ground Pound Range (Tank)
```

---

## 🐛 Yaygın Sorunlar

### Problem 1: AI Hareket Etmiyor
```
Çözüm:
✓ Rigidbody2D var mı?
✓ Gravity Scale = 0 mı?
✓ AITokenManager sahnede mi?
✓ Player tag'i "Player" mı?
```

### Problem 2: Saldırı Hiçbir Zaman Gelmiyor
```
Çözüm:
✓ Token sistemi çalışıyor mu? (max 2 attacker)
✓ Attack Range yeterli mi?
✓ Animator'da "Attack" trigger var mı?
✓ Animation Event eklendi mi?
```

### Problem 3: Hitbox Çalışmıyor
```
Çözüm:
✓ Hitbox.cs bağlı mı?
✓ Target Layer ayarlandı mı? (Player layer)
✓ Animation Event doğru frame'de mi?
✓ ActivateAttackHitbox() metodu çağrılıyor mu?
```

### Problem 4: Tank Duvara Çarpmıyor
```
Çözüm:
✓ Duvarların Tag'i "Wall" mi?
✓ Duvar collider'ları var mı?
✓ Physics2D Layer Matrix ayarları?
```

---

## ⚖️ Balancing (Dengeleme)

### Difficulty Ayarları

**Easy Mode:**
```csharp
// Tüm AI'lar için
detectionRange *= 0.7f;
attackCooldown *= 1.5f;
aggressiveness = 0.3f;
```

**Normal Mode:**
```csharp
// Default values
```

**Hard Mode:**
```csharp
detectionRange *= 1.3f;
attackCooldown *= 0.7f;
aggressiveness = 0.9f;

// Fast Dodger
dodgeChance = 0.6f;

// Heavy Tank
berserkerThreshold = 0.5f; // Daha erken berserker
```

---

## 📊 Karşılaştırma Tablosu

| Özellik | Basic Brawler | Fast Dodger | Heavy Tank |
|---------|--------------|-------------|------------|
| **HP** | 50 | 40 | 150 |
| **Speed** | 3.0 | 6.0 | 2.0 |
| **Damage** | 10-15 | 12 | 25-30 |
| **Threat (Solo)** | ⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Threat (Group)** | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Counter** | Combo chain | Grab, corner | Dodge timing, walls |
| **AI Complexity** | Simple | Medium | Complex |

---

## 🎯 Spawn Önerileri

### Level 1 (Easy)
```
Wave 1: 3x Basic Brawler
Wave 2: 4x Basic Brawler
Wave 3: 2x Basic Brawler + 1x Fast Dodger
```

### Level 2 (Medium)
```
Wave 1: 2x Basic Brawler + 1x Fast Dodger
Wave 2: 3x Fast Dodger
Wave 3: 4x Basic Brawler + 1x Heavy Tank
```

### Level 3 (Hard)
```
Wave 1: 1x Heavy Tank + 2x Fast Dodger
Wave 2: 2x Heavy Tank
Wave 3: 1x Heavy Tank + 4x Basic Brawler
```

### Boss Fight
```
Main Boss + 2x Heavy Tank + 4x Fast Dodger
(Waves of Basic Brawlers spawn continuously)
```

---

## 🔄 Runtime Customization

### AI Kişiliğini Değiştirme

```csharp
// Inspector'dan runtime'da değiştirilebilir
enemy.GetComponent<BasicBrawlerAI>().aggressiveness = 1.0f;
enemy.GetComponent<FastDodgerAI>().dodgeChance = 0.8f;
enemy.GetComponent<HeavyTankAI>().ActivateBerserkerMode();
```

### Dynamic Difficulty

```csharp
void AdjustDifficulty(float playerPerformance)
{
    foreach (var enemy in FindObjectsOfType<EnemyAIBase>())
    {
        if (playerPerformance > 0.8f) // Player iyi
        {
            enemy.aggressiveness = 0.9f;
            enemy.intelligence = 1.0f;
        }
        else if (playerPerformance < 0.4f) // Player zor durumda
        {
            enemy.aggressiveness = 0.4f;
            enemy.intelligence = 0.5f;
        }
    }
}
```

---

## 📚 İleri Seviye

### Custom AI Yaratma

```csharp
// EnemyAIBase'den türet
public class MyCustomAI : EnemyAIBase
{
    protected override void UpdateAI()
    {
        // Kendi behavior tree logic'iniz
    }
    
    // Custom attack patterns
    private IEnumerator MySpecialAttack()
    {
        // Implementation
    }
}
```

### Hybrid Behaviors

```csharp
// Örn: Basic Brawler + Fast Dodger karışımı
public class AgileB rawlerAI : BasicBrawlerAI
{
    [SerializeField] private float dodgeChance = 0.2f;
    
    public override void OnDamageReceived()
    {
        if (Random.value < dodgeChance)
        {
            // FastDodger'dan dodge behavior ödünç al
            StartCoroutine(QuickDodge());
        }
        else
        {
            base.OnDamageReceived();
        }
    }
}
```

---

## ✅ Checklist

### Prefab Hazır mı?
- [ ] Rigidbody2D (Gravity 0, Freeze Z)
- [ ] Colliders (Hurtbox + Hitbox)
- [ ] Animator Controller bağlı
- [ ] AI Script eklenmiş
- [ ] SortingOrderController var

### Animator Hazır mı?
- [ ] Tüm state'ler oluşturulmuş
- [ ] Parameters eklenmiş
- [ ] Transitions ayarlanmış
- [ ] Animation Events eklenmiş

### Sahne Hazır mı?
- [ ] AITokenManager var
- [ ] Player tag'i doğru
- [ ] Walls tag'i doğru (Tank için)
- [ ] Layer Matrix ayarlandı

### Test Edildi mi?
- [ ] Detection çalışıyor
- [ ] Chase çalışıyor
- [ ] Attack çalışıyor
- [ ] Token sistemi çalışıyor
- [ ] Damage alıyor/veriyor

---

**Artık 3 farklı düşman AI'sı kullanıma hazır!** 🎮⚔️

**Kolay gelsin!** 🚀

