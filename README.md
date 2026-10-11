# ⚔️ A Knight With Two Guns Trapped Without a Sun

> A 3D third-person wave shooter where a tiny knight, a rifle, and a railgun face an endless skeleton army that keeps getting angrier.

A fast, arcade-style **3D roguelike wave shooter** built in **Unity 6**. You are a knight stuck in a dark arena with two guns: a hold-to-fire **rifle** and a charge-up **railgun**. Dash through attacks, slam the ground with a **dive attack**, and survive as each wave brings more skeletons, and tougher ones, than the last.

<!-- Add a screenshot or GIF here, e.g.
![Gameplay](docs/gameplay.gif)
-->
<img width="320" height="180" alt="image" src="https://github.com/user-attachments/assets/f1e57528-9c79-43d2-aa36-9e1c08717bd9" />
<img width="320" height="180" alt="image" src="https://github.com/user-attachments/assets/e072af1d-7aef-4529-a9a0-7d844c03add2" />
<img width="320" height="180" alt="image" src="https://github.com/user-attachments/assets/021abe1a-53eb-49b5-b3f4-aac878ce6f06" />


---

## ✨ Features

- **Two weapons you swap on the fly**: a rapid-fire rifle and a piercing, charge-up railgun
- **Dash** with directional animations, a glowing afterimage trail, and a camera FOV kick
- **Dive attack**: jump, then press jump again to slam down in an area-of-effect explosion with hit-stop, camera shake, a ground decal, and knockback
- **Over-the-shoulder aiming** with a right-click zoom and reduced look sensitivity
- **Five skeleton enemy types**, including a stealthy **rogue** that only shows itself when you get close
- **Wave system with scaling difficulty**: enemy speed, damage, and health grow every wave
- **Floating damage numbers**, impact particles, muzzle flashes, and railgun shockwaves
- **HUD** with health bar, wave counter, enemy count, score, enemy stat increments, and a **minimap**
- Smooth movement with **coyote time** and **jump buffering**

---

## 🎮 Controls

| Action | Input |
| --- | --- |
| Move | `W` `A` `S` `D` |
| Look | Mouse |
| Jump | `Space` (release early for a shorter hop) |
| **Dive attack** | `Space` again while in the air |
| Dash | `Left Shift` (dashes in your movement direction, or forward if standing still) |
| Shoot | Left click (hold to fire the rifle, or hold to charge and fire the railgun) |
| Aim / zoom | Right click |
| Switch weapon | Mouse scroll wheel |

> The cursor is locked during play. Bindings live in `Assets/InputSystem_Actions.inputactions` (Unity's Input System), so they're easy to change.

---

## 🔫 Weapons & Abilities

### Rifle
Hold left click for full-auto fire. Bullets travel toward whatever is under your crosshair (a camera ray finds the target point), while a separate visual tracer flies from the gun barrel. Each hit does 1 damage and the gun has a small recoil kick.

### Railgun
Hold left click to **charge**; it fires every time the charge completes while you keep holding. The shot **pierces through everything**, deals 100 damage (instakilling any skeleton), shoves nearby physics objects, kicks you backwards, and shakes the camera. You move slower while charging, so pick your moment.

### Dash
A short burst of speed that plays a different animation for forward, back, left, and right. You pass through enemies while dashing, and it leaves a glowing ghost trail. It has a cooldown.

### Dive Attack
Jump and press `Space` again to freeze for a moment, then slam into the ground. The impact:
- damages every enemy in range (**up to 9 damage close in, falling off to 0.5 at distance**)
- launches them with an explosion force
- triggers hit-stop, camera shake, and a shockwave decal

---

## 💀 Enemies

All enemies chase you, spread out around your position (each picks a random waypoint near the player so they surround you instead of stacking), and swing when you're in range. Getting hit knocks you back and interrupts your actions.

| Enemy | Health | Speed | Damage | Score | Notes |
| --- | --- | --- | --- | --- | --- |
| **Minion** | 10 | 1 | 1 | 10 | The basic grunt and the only enemy in wave 1 |
| **Rogue** | 5 | 3 | 1 | 15 | **Invisible**; only flashes into view for a few seconds when you get close. Fast, long reach, heavy knockback |
| **Warrior (Axe)** | 20 | 1 | 3 | 20 | Slow and hits hard |
| **Warrior (Sword)** | 20 | 1 | 2 | 30 | Long reach |
| **Warrior (Spear)** | 20 | 2 | 1 | 35 | Fast, with strong knockback |

*(Base values from the prefabs. From wave 4 onward they scale up, see below.)*

---

## 🌊 Waves & Difficulty

- **Wave 1** has minions only, spawned slowly in a ±30 unit area.
- **Wave 2** mixes in more enemy types and spawns faster.
- **Wave 3** opens up the arena (±50) and uses the full roster with a 20-second pause afterwards.
- **Wave 4+** is endless: the next wave starts **as soon as the last enemy is dead**, spawning across a huge ±75 area.

Number of enemies per wave: `((0.002w + 0.05)w + 1)w + 1`

From wave 4 onward every enemy also gets bonus stats added on top of its base values (shown live in the top-left HUD):

| Stat | Bonus added |
| --- | --- |
| Speed | `(2.5 × ln(w) + 2) / 2` |
| Damage | `(0.8w + √w) / 2` |
| Health | `0.05w² + 0.1w + 2` |

Score goes up for each kill. If your health hits zero you are sent back to the main menu.

---

## 🚀 Getting Started

### Requirements

- **Unity 6000.5.4f1** (Unity 6.5). Using the exact version avoids upgrade prompts.
- Universal Render Pipeline 17.5
- Packages (pinned in `Packages/manifest.json`): Input System 1.19, **Cinemachine 3.1.7**, AI Navigation 2.0, TextMeshPro, Timeline

### Run it

```bash
git clone https://github.com/Mehran579/3droguelikerepo.git
```

1. Open **Unity Hub** → **Add** → select the cloned folder.
2. Let Unity import everything (there are a lot of assets, so the first import takes a while).
3. Open `Assets/_Recovery/Start menu.unity` and press **Play**.

Build Settings scene order:

| Index | Scene |
| --- | --- |
| 0 | `Start menu` |
| 1 | `game scene` |

Both scenes are in `Assets/_Recovery/`.

---

## 🧠 How It Works

### Player (`playermanager`)
One script drives the whole player: movement relative to the camera, jumping with coyote time and an input buffer, the dash, the dive, both weapons, and aim zoom. Camera effects (FOV changes, shakes) go through **Cinemachine**, and knockback briefly takes control away from the player.

### Bullets
Both guns use **two bullets**: a *real* bullet fired from the camera toward the crosshair target (so shots go exactly where you aim), and a *visual* bullet fired from the gun barrel toward the same point (so it looks right). This keeps aiming accurate with an over-the-shoulder camera.

### Enemies
- `enemy_manager`: rotates to face the player, walks toward a personal waypoint around the player, triggers attack animations in range, and takes damage from bullets
- `hit`: sits on the weapon collider and deals damage and knockback when it touches the player
- `healthmanager`: shared health for the player and enemies, floating damage numbers, death animations (one of four chosen at random)
- `rogue`: toggles the renderers so the enemy is invisible except for a short window when you're close
- `score_increaser`: awards score and updates the enemy counter when an enemy dies

### Waves (`wave`)
Spawns each wave, tracks living enemies through `scoremanager`, and applies the stat scaling formulas to newly spawned enemies.

### Effects
`dashtrailplayer` bakes the skinned meshes into temporary ghost meshes with a fading glowing material for the dash and dive trails. `rail_gun_ripples` spawns rapid shockwave rings, and `railgunbulletbehaviour` handles the piercing shot and its physics push.

---

## 📁 Project Structure

```
Assets/
├── Scripts/                 # All gameplay code
├── PreFabs/                 # Bullets, shockwave, damage numbers
│   └── enemeies/            # Minion, Rogue, Warrior (Axe / Sword / Spear)
├── _Recovery/               # Scenes: Start menu, game scene
├── animations/              # Animator controllers and clips (player, skeletons, dive explosion)
├── characs/                 # Knight model and the custom "desolate dive" animation (.blend)
├── shaders/                 # Dash/dive trail shader graph, railgun charge, minimap materials
├── HandRailGun/             # Railgun model
├── 4K 3D Weapons Mega Pack/ # Rifle model
├── JMO Assets/              # WarFX and Cartoon FX particle effects
├── zipped assets/           # KayKit source packs
└── InputSystem_Actions.inputactions
```

### Script cheat sheet

| Script | What it does |
| --- | --- |
| `playermanager` | Movement, jump, dash, dive, rifle, railgun, aim, knockback |
| `wave` | Wave spawning and difficulty scaling |
| `enemy_manager` | Enemy movement, attacks, and bullet damage |
| `hit` | Enemy weapon hitbox (damage and knockback to the player) |
| `healthmanager` | Health, damage numbers, death handling |
| `rogue` | Stealth and reveal behaviour |
| `scoremanager` / `score_increaser` | Score and remaining enemy count |
| `railgunbulletbehaviour` / `rail_gun_ripples` | Railgun projectile and shockwave visuals |
| `dashtrailplayer` | Afterimage trails for dash and dive |
| `planemover` | Keeps the ground plane under the player |
| `game_load` | Start menu "Play" button and loading panel |

---

## 🛠️ Built With

- [Unity 6](https://unity.com/) with the Universal Render Pipeline
- Cinemachine, Input System, Shader Graph, TextMeshPro
- C#
- Blender (custom dive animation)

---

## 🙏 Credits

- **Characters, skeletons, animations, and weapon bits:** [KayKit](https://kaylousberg.itch.io/) by Kay Lousberg (CC0): *Adventurers*, *Skeletons*, *Character Animations*, and *Fantasy Weapons Bits*
- **Particle effects:** WarFX and Cartoon FX by Jean Moreno (JMO)
- **Rifle model:** 4K 3D Weapons Mega Pack
- **Railgun model:** HandRailGun
- **Crosshair icon:** Pngtree
- Everything else by **Mehran**

---

## 📌 Status

Playable with endless waves, two weapons, and a full move set. Bug reports, balance ideas, and new enemy suggestions are welcome. Open an issue or a pull request!
