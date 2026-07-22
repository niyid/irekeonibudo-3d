# Irekeonibudo 3D — Unity Prototype

Sibling project to `ogbojuode-3d`, same engine and architecture, different
story: **Ìrèké Oníbùdó** (1949) by D.O. Fagunwa — a shipwrecked traveler
swept into an undersea kingdom ruled by the mermaid queen **Arògìdígbà**,
guided through her trials by his dead mother's spirit.

Real, working project files — no encoded blobs, no tricks.

## Setup

1. Install Unity Hub + Editor with Android Build Support (see
   `ogbojuode-3d`'s setup notes — identical process, same toolchain).
2. In Unity Hub: **New Project → 3D (URP)** → name it `IrekeOnibudo3D`.
3. Close Unity. Copy this folder's `Assets/` directory into the new
   project's `Assets/` folder.
4. Reopen the project, let it recompile.
5. Click **Ireke Onibudo → Build Entire Universe** in the menu bar. This
   auto-creates the `Player`/`Enemy` tags and assembles:
   - **Ilu Eti-Odo** (riverside hub) — huts, a dock, a canoe, a shrine to
     Olókun lit like a bonfire, a reed barrier at the water's edge.
   - **Ijoba Omi** (undersea kingdom) — coral reef stand-ins, glowing
     plankton, stretching north from the hub same as ogbojuode-3d's forest.
   - **Three creatures drawn from the actual book**: the flying snake that
     seizes a princess (fast/fragile), the Wrestler-Cat from the
     story-within-a-story (slow/brutal), and a warrior-fish guard of
     Arogidigba's kingdom (towering).
   - **Arogidigba**, the mermaid queen — a riddle encounter matching the
     source material's "trials designed for failure," not a straight
     fight. Hostile only if her trial is failed.
   - **Three wandering visitations of Ireke Onibudo's mother's spirit** —
     his actual guide through the trials in the book, not a generic NPC.
   - Wisdom/expedition tracking, camera, lighting.
6. Press **Play**. Controls: WASD to move, left-click to swing the blade
   (Idà), right-click to throw a spear (assign a `spearPrefab` — any small
   object with a Rigidbody works), **E** to cast Egbe (teleport forward),
   **F** near a spirit or Arogidigba to hear their riddle/trial.

   Same caveat as ogbojuode-3d: `RiddleGiver` currently auto-resolves as
   "correct" on interact — no answer-input UI yet.

## Everything here is still primitive geometry

No models imported yet. See **`irekeonibudo-meshy_prompts.txt`** for the
full prompt list to generate the seven custom character/creature/prop
assets — same workflow as `ogbojuode-3d`: Tripo AI (or Meshy) for the
custom cast, Kenney/Poly Haven CC0 kits for generic environment props
(huts, dock planks, reeds), reserving AI-generation credits for the
handful of assets a generic library can't cover.

Once assets exist, extend `SceneSetupWizard.cs` with the same
load-real-asset-with-primitive-fallback pattern used in `ogbojuode-3d`
(`AssetDatabase.FindAssets("t:Model", folder)` per character folder, exact
paths for props) — that code is directly portable between the two
projects with just the asset-path constants changed.

## Shared vs. story-specific code

To keep both sibling projects easy to maintain together, some scripts are
copied verbatim (identical logic, safe to diff against `ogbojuode-3d`) and
some are reflavored for this story:

**Copied unchanged** (fully story-agnostic):
- `IDamageable.cs`
- `CreatureAI.cs` — role-based preset enum (`Titan`/`Brute`/`Swift`)
  rather than either story's specific creature names, so it's genuinely
  shared rather than coincidentally identical
- `WisdomTracker.cs`
- `RiddleGiver.cs` — calls `OnRiddleFailed()` via `SendMessage` rather
  than referencing a specific boss class, so it works with either
  `OstrichKingBoss` or `ArogidigbaBoss` unmodified

**Reflavored for this story:**
- `IrekeOnibudoController.cs` (was `YorubaHunterController.cs`) — thrown
  spear instead of musket, blade instead of machete, Egbe unchanged
- `ExpeditionManager.cs` — hub-vs-kingdom framing instead of
  hub-vs-forest, boundary constant matched to this project's layout
- `PlayerVitals.cs` — same charm-system stub, defeat message no longer
  hardcodes the other story's protagonist's name
- `ArogidigbaBoss.cs` (new, mirrors `OstrichKingBoss.cs`'s pattern)
- `MotherSpiritGuide.cs` (new, mirrors `GhommidSpirit.cs`'s pattern)

## File map

- `Assets/Scripts/IrekeOnibudoController.cs` — player movement, blade,
  thrown spear, Egbe teleport.
- `Assets/Scripts/IDamageable.cs` — shared damage interface.
- `Assets/Scripts/PlayerVitals.cs` — player health, charm-system stub.
- `Assets/Scripts/CreatureAI.cs` — one AI, three role presets.
- `Assets/Scripts/ArogidigbaBoss.cs` — the mermaid queen; riddle-gated
  hostility, same pattern as the Ostrich-King.
- `Assets/Scripts/MotherSpiritGuide.cs` — wandering guide-spirit movement.
- `Assets/Scripts/RiddleGiver.cs` — riddle/interact logic, story-agnostic.
- `Assets/Scripts/WisdomTracker.cs` — singleton wisdom tracker.
- `Assets/Scripts/ExpeditionManager.cs` — hub-vs-kingdom state tracking.
- `Assets/Scripts/MobileTouchUI.cs` — virtual joystick + action buttons.
- `Assets/Editor/SceneSetupWizard.cs` — editor menu tool
  (`Ireke Onibudo → Build Entire Universe`).
