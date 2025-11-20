# 🤖 ENEMY AI DESIGN - Düşman Yapay Zeka Tasarımı

Crazy Flasher tarzında 3 farklı düşman AI'sı.

---

## 📋 İçindekiler

- [Basic Brawler](#basic-brawler)
- [Fast Dodger](#fast-dodger)
- [Heavy Tank](#heavy-tank)
- [Implementation Guide](#implementation-guide)

---

# 1. BASIC BRAWLER (Sokak Kavgacısı)

## 🎯 Karakter Profili

**Tip**: Temel düşman, sayıca çok  
**Taktik**: Kalabalık halde tehdit, basit saldırılar  
**Zayıflık**: Düşük can, öngörülebilir  

### Stats
```
HP: 50
Speed: 3.0
Damage: 10
Attack Range: 1.5
Detection Range: 8.0
Attack Cooldown: 2.0s
Knockback Resistance: 1.0 (Normal)
```

---

## 🌳 Davranış Ağacı (Behavior Tree)

```
Root (Selector)
├── [Dead?] → PlayDeathAnimation
│
├── [Has Attack Token?]
│   └── Sequence
│       ├── [In Attack Range?]
│       │   └── Selector
│       │       ├── [Behind Player?] → BackAttack
│       │       └── [Front of Player?] → FrontCombo
│       │
│       └── [Not In Range?]
│           └── ChasePlayer
│
└── [No Token?]
    └── Selector
        ├── [Too Close?] → BackOff
        └── [Normal Distance?] → CircleStrafe
```

---

## 🔄 Durum Makinesi (State Machine)

### States:

#### **1. Idle**
```
Behavior: Bekle, etrafı izle
Duration: 0.5-1.5 saniye
Transition:
  → Patrol (Random zamanlayıcı)
  → Chase (Oyuncu detection range'de)
```

#### **2. Patrol**
```
Behavior: Yavaş yavaş rastgele yönlerde dolaş
Speed: 1.5 (moveSpeed * 0.5)
Transition:
  → Chase (Oyuncu tespit edildi)
  → Idle (Random zamanlayıcı)
```

#### **3. Chase**
```
Behavior: Oyuncuya doğru koş
Speed: 3.0 (normal)
Transition:
  → Attack (Token var + saldırı menzili)
  → CircleStrafe (Token yok)
  → Idle (Oyuncu çok uzakta)
```

#### **4. CircleStrafe**
```
Behavior: Oyuncunun etrafında dolaş (tehdit edici)
Pattern: Sin wave movement
Transition:
  → Chase (Token aldı)
  → BackOff (Çok yakın)
```

#### **5. Attack**
```
Behavior: Saldırı combo yap
Duration: 0.8 saniye
Sub-states:
  - WindUp (0.2s) → Preparing
  - Strike (0.3s) → Hitbox active
  - Recovery (0.3s) → Vulnerable
Transition:
  → Chase (Saldırı tamamlandı)
  → Hurt (Hasar aldı)
```

#### **6. BackOff**
```
Behavior: Oyuncudan geri çekil
Duration: 1.0 saniye
Speed: -2.0 (backwards)
Transition:
  → CircleStrafe (Güvenli mesafe)
```

#### **7. Hurt**
```
Behavior: Hasar alma animasyonu, kısa stun
Duration: 0.4 saniye
Invulnerable: false
Transition:
  → Death (HP <= 0)
  → Chase (Stun bitti)
```

#### **8. Death**
```
Behavior: Ragdoll aktif, collider kapalı
Duration: 3 saniye → Destroy
```

---

## ⚔️ Saldırı Patternleri

### Pattern 1: Front Combo (70% şans)
```
1. Step Forward (0.1s)
2. Jab (Light Punch)
   - Damage: 10
   - Hitbox Active: 0.1s
3. Wait (0.2s)
4. Cross (Heavy Punch)
   - Damage: 15
   - Hitbox Active: 0.15s
5. Recovery (0.3s)
```

### Pattern 2: Back Attack (30% şans)
```
1. Quick Step Behind (Teleport benzeri)
2. Grab Attempt
   - Success: Throw player
   - Fail: Stunned 0.5s
```

### Pattern 3: Desperate Attack (HP < 30%)
```
Behavior: Sürekli saldırı (berserk)
Cooldown: 0.5s (daha hızlı)
Risk/Reward: Daha agresif, daha vulnerable
```

---

# 2. FAST DODGER (Hızlı Kaçan)

## 🎯 Karakter Profili

**Tip**: Sinir bozucu, hit-and-run  
**Taktik**: Vurup kaç, dodge spam  
**Zayıflık**: Düşük can, grab edilirse zor durumda  

### Stats
```
HP: 40
Speed: 6.0 (2x normal)
Damage: 12
Attack Range: 1.2
Detection Range: 10.0
Attack Cooldown: 1.5s
Dodge Chance: 40%
Knockback Resistance: 0.7 (Lighter)
```

---

## 🌳 Davranış Ağacı

```
Root (Selector)
├── [Dead?] → PlayDeathAnimation
│
├── [Player Attacking?]
│   └── [40% Chance] → DodgeRoll
│
├── [Has Attack Token?]
│   └── Sequence
│       ├── HitAndRun
│       │   ├── DashIn
│       │   ├── QuickStrike
│       │   └── DashOut
│       │
│       └── [Player Close?] → BackflipAway
│
└── [No Token?]
    └── KeepDistance (Kite player)
```

---

## 🔄 Durum Makinesi

### States:

#### **1. Observe**
```
Behavior: Mesafe koru, oyuncuyu izle
Distance: 5-7 birim
Transition:
  → DodgeRoll (Oyuncu saldırıyor)
  → HitAndRun (Token aldı)
```

#### **2. HitAndRun**
```
Sequence:
  1. DashIn (0.2s) - Hızlı yaklaş
  2. QuickStrike (0.3s) - Tek vuruş
  3. DashOut (0.2s) - Hızlı uzaklaş
Total: 0.7s
```

#### **3. DodgeRoll**
```
Behavior: İ-frame dodge (0.3s)
Direction: Away from player
Speed: 10.0
Invulnerable: true
Transition:
  → Observe (Dodge tamamlandı)
```

#### **4. KeepDistance**
```
Behavior: Oyuncudan kaç
Kite Pattern: Backward movement + random strafe
Speed: 6.0
Transition:
  → DodgeRoll (Oyuncu çok yaklaştı)
  → HitAndRun (Token + güvenli mesafe)
```

#### **5. Backflip**
```
Behavior: Akrobatik geri dönüş
Distance: 3 birim
Duration: 0.5s
Visual: Stylish animation
Transition:
  → Observe
```

#### **6. Cornered**
```
Behavior: Köşeye sıkışmış, desperate
Pattern: Spam dodge + wild attacks
Trigger: Wall behind + player close
Risk: Daha vulnerable ama aggressive
```

---

## ⚔️ Saldırı Patternleri

### Pattern 1: Hit-and-Run (Ana taktik)
```
1. Dash In (0.2s)
   - Speed boost x2
   - Trail effect
2. Quick Stab (0.15s)
   - Damage: 12
   - Fast hitbox
3. Dash Out (0.2s)
   - Invulnerable
   - Backward movement
```

### Pattern 2: Feint Attack (20% şans)
```
1. Fake Dash In (0.1s)
2. Stop abruptly
3. Wait for player reaction
4. Punish (if player whiffed)
```

### Pattern 3: Desperation Combo (HP < 20%)
```
Behavior: Abandon hit-and-run
1. Dash In
2. Multi-hit combo (3 hits)
3. No dash out (commit)
Risk: High damage but vulnerable
```

---

# 3. HEAVY TANK (Ağır Zırhlı)

## 🎯 Karakter Profili

**Tip**: Boss-like mini-tank  
**Taktik**: Super armor, knockback resistant, high damage  
**Zayıflık**: Yavaş, dodge edilebilir, uzun recovery  

### Stats
```
HP: 150
Speed: 2.0 (Yavaş)
Damage: 25
Attack Range: 2.0
Detection Range: 6.0
Attack Cooldown: 3.0s
Armor: Super Armor (Flinch resistance)
Knockback Resistance: 3.0 (Very High)
```

---

## 🌳 Davranış Ağacı

```
Root (Selector)
├── [Dead?] → PlayDeathAnimation
│
├── [HP < 30%?]
│   └── BerserkerMode
│       ├── ChargeAttack
│       └── GroundPound (AOE)
│
├── [Has Attack Token?]
│   └── Selector
│       ├── [Close Range?] → Grab
│       ├── [Medium Range?] → HeavySwing
│       └── [Far Range?] → Charge
│
└── [No Token?]
    └── SlowAdvance (Intimidating walk)
```

---

## 🔄 Durum Makinesi

### States:

#### **1. Intimidate**
```
Behavior: Yavaş yavaş yaklaş (tehdit edici)
Speed: 2.0
Animation: Heavy breathing, flexing
Transition:
  → Charge (Token + far away)
  → HeavySwing (Token + close)
```

#### **2. Charge**
```
Behavior: Hızlı koşu saldırısı (boğa gibi)
Phases:
  1. Wind-up (0.5s) - Warning
  2. Charge (1.0s) - Speed x3, super armor
  3. Crash (0.3s) - Stop, stunned if miss wall
Damage: 25
Hitbox: Large, frontal
Transition:
  → Stunned (Duvara çarptı)
  → Recovery (Hit player)
```

#### **3. HeavySwing**
```
Behavior: Yavaş ama güçlü saldırı
Phases:
  1. Wind-up (0.6s) - Telegraph
  2. Swing (0.4s) - Hitbox active
  3. Recovery (0.5s) - Vulnerable
Damage: 25
Knockback: High
Transition:
  → Intimidate (Recovery bitti)
```

#### **4. Grab**
```
Behavior: Yakala ve fırlat
Range: 1.5 birim
Success:
  1. Grab (0.2s)
  2. Lift (0.3s)
  3. Throw (0.5s) - Damage: 30
Fail:
  → Whiff recovery (1.0s) - Very vulnerable
```

#### **5. GroundPound (AOE)**
```
Trigger: HP < 30% OR player behind
Behavior: Yere yumruk (shockwave)
Phases:
  1. Jump (0.3s)
  2. Pound (0.4s)
  3. Shockwave (0.3s) - Radial damage
Damage: 20
Range: 3 birim (circle)
Stun: 0.5s (oyuncuyu da)
```

#### **6. Berserker**
```
Trigger: HP < 30%
Behavior: Daha hızlı, daha agresif
Speed: 3.0 (boost)
Attack Cooldown: 1.5s (reduced)
Super Armor: Always active
Risk: Takes more damage
```

#### **7. Stunned**
```
Trigger: Charge miss wall
Duration: 2.0s
Vulnerable: Very (takes 2x damage)
Transition:
  → Intimidate (Stun bitti)
```

---

## ⚔️ Saldırı Patternleri

### Pattern 1: Heavy Swing (Ana saldırı)
```
1. Wind-up (0.6s)
   - Visual: Glow effect, telegraph
   - Audio: Heavy breathing
2. Swing (0.4s)
   - 180 degree arc
   - Damage: 25
   - Knockback: 10 units
3. Recovery (0.5s)
   - Vulnerable window
```

### Pattern 2: Charge Attack (Uzak mesafe)
```
1. Roar (0.3s) - Warning
2. Charge (1.0s)
   - Speed: 6.0 (3x boost)
   - Super armor
   - Trail effect
3. Impact
   - Hit: Massive knockback
   - Miss: Stunned 2s
```

### Pattern 3: Ground Pound (Desperation)
```
1. Jump (0.3s)
2. Peak (0.2s)
3. Slam (0.4s)
4. Shockwave
   - Inner radius: 2 units (30 damage)
   - Outer radius: 3 units (15 damage)
   - Stun: 0.5s
```

### Pattern 4: Grab Combo (Close range)
```
1. Grab attempt (0.2s)
   - Success rate: 60%
2. If success:
   - Lift (0.3s)
   - Throw (0.5s) - 30 damage
3. If fail:
   - Whiff animation (1.0s)
   - Very vulnerable
```

---

# IMPLEMENTATION GUIDE

## 🏗️ Mimari Yaklaşım

### Hybrid System (Mevcut ile uyumlu)

```
EnemyAIBase (Abstract)
  ├── BasicBrawlerAI
  ├── FastDodgerAI
  └── HeavyTankAI

Each implements:
  - State Machine (Class-based)
  - Behavior Tree logic (Update method)
  - Attack Patterns (Coroutines)
```

---

## 📊 AI Decision Flow

### Her Frame:
```
1. Check vital conditions (Dead? Hurt?)
2. Token system check
3. Behavior tree evaluation
4. State machine update
5. Animation update
6. Physics update
```

---

## 🎯 Balancing Guidelines

### Basic Brawler
```
Threat Level: ★☆☆☆☆ (Solo)
Threat Level: ★★★★☆ (Group)
Counter: Kombo yapmak, AOE saldırılar
```

### Fast Dodger
```
Threat Level: ★★★☆☆ (Solo)
Threat Level: ★★★☆☆ (Group)
Counter: Grab, timing saldırıları, köşeye sıkıştırma
```

### Heavy Tank
```
Threat Level: ★★★★★ (Solo)
Threat Level: ★★★★☆ (Group) - Yavaş oldukları için
Counter: Dodge timing, charge'ı duvara çarptırma, recovery windows
```

---

## 🎮 Player Counterplay

### Basic Brawler'a Karşı:
```
✅ Kombo ile chain kill
✅ AOE saldırılar
✅ Hızlı hareket
❌ Tek tek uğraşmak
```

### Fast Dodger'a Karşı:
```
✅ Grab saldırıları
✅ Köşeye sıkıştırma
✅ Feint attacks
❌ Spam saldırı
```

### Heavy Tank'e Karşı:
```
✅ Dodge timing
✅ Charge'ı wall'a çarptır
✅ Recovery window'ları kullan
❌ Direkt kavga
```

---

## 🔧 Tuning Parameters

Her düşman için ayarlanabilir:

```csharp
[Header("AI Tuning")]
public float aggressiveness = 0.5f;      // 0-1
public float caution = 0.3f;              // 0-1
public float intelligence = 0.7f;         // 0-1 (Feint usage)
public float teamwork = 0.6f;             // 0-1 (Coordination)
```

---

## 📈 Difficulty Scaling

### Easy Mode:
```
- Detection range -30%
- Attack cooldown +50%
- Damage -25%
- Dodge chance -50%
```

### Hard Mode:
```
- Detection range +50%
- Attack cooldown -30%
- Damage +50%
- Dodge chance +50%
- Perfect timing attacks
```

---

**Şimdi kodları yazıyorum!** 🚀

