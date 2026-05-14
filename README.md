# RISE of the Bear

## Project Description

RISE of the Bear is a single-player first-person shooter built in Unity 6 for CS4700. The game takes place inside an arcade claw machine where the player controls a teddy bear with a toy gun. The player must fight evil teddy bears, survive a dangerous claw hazard, collect pickups, and escape through the prize chute.

The project is designed as a vertical slice FPS prototype that demonstrates raycast shooting, enemy AI, NavMesh movement, pickups, UI, camera feedback, and a complete win/lose gameplay loop.

## Controls

| Action | Input |
|---|---|
| Move | WASD |
| Look | Mouse |
| Shoot | Left Mouse Button |
| Jump | Space |
| Interact with UI | Mouse |

## Features Implemented

- First-person player controller
- Raycast-based shooting
- Muzzle flash and bullet trail visual feedback
- Ammo system with live ammo UI
- Health system with live health UI
- Ammo pickup
- Health pickup
- Claw Stop pickup
- Two enemy types:
  - Evil Teddy
  - Evil Bunny
- Enemy NavMesh AI with patrol, chase, attack, and death behavior
- Enemy attack animation event using `DealAttackDamage()`
- Claw hazard that tracks the player
- Red warning indicator before claw drop
- Claw drop, damage, grab, lift, and release behavior
- Claw safe zone inside tunnel areas
- Prize chute exit system
- Exit unlocks after all enemies are defeated
- Level transitions
- Main menu with Play and Quit buttons
- Win and lose screens
- Restart and Quit buttons
- Camera shake when shooting
- ScriptableObject weapon stats
- Background music and sound effect fields

## Gameplay Goal

Defeat all evil teddy bears in each level while avoiding the claw hazard. Collect ammo and health pickups to survive. Use the Claw Stop pickup to temporarily disable the claw. Once all enemies are defeated, the prize chute unlocks and allows the player to move to the next level or win the game.

## Build Instructions

1. Open the project in Unity 6.
2. Open the `MainMenu` scene.
3. Go to `File > Build Profiles`.
4. Make sure the scenes are included in this order:
   - MainMenu
   - Level 1
   - Level 2
   - Level 3
5. Select the desired platform.
6. Click `Build`.
7. Run the generated build.

## WebGL Instructions

For the WebGL version:

1. Go to `File > Build Profiles`.
2. Select the Web build profile.
3. Use `Desktop - Release`.
4. Click `Build > Clean Build`.
5. Upload the generated WebGL ZIP to itch.io as an HTML game.

## Known Issues

- Some audio clips may still need final balancing.
- Enemy behavior depends on properly baked NavMesh surfaces in each level.
- WebGL performance may vary depending on browser and computer hardware.
- Some models and materials may appear slightly different in the browser build than in the Unity editor.

## Asset Attributions

This project uses a combination of self-made assets, Unity tools, and third-party assets.

Assets/tools used include:

- Unity 6
- Unity Starter Assets First Person Controller
- Unity NavMesh / AI Navigation
- Unity ProBuilder
- TextMeshPro
- Meshy AI for generated 3D game assets
