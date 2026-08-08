# Architecture

Five layers, arranged as a compiler stack where the source language is *intent* and the
target is a load order. AI lives in layers 1–3. Layers 4–5 are deterministic engineering.

## 1. World model (knowledge layer)

A knowledge graph (kgrdbms ontology, e.g. `fo4`) holding:

- **Vanilla ground truth** — quests, factions, NPCs, locations, items, lore. Ingested from
  plugin records (via Mutagen extraction) and wiki sources.
- **Generated canon** — every mod this system has produced: its NPCs, its quests, its lore.
  This is what makes the game *forever*: new content can reference, extend, and consequence
  old generated content.
- **Player history** — the distilled narrative of the playthrough, updated from save telemetry.

Design rule: the KG stores *facts and relationships*, not prose. Prose is generated fresh
from facts at generation time.

## 2. Game-state access (MCP layer)

An MCP server (`fo4`) — structural twin of the in-house `eu5` server — exposing:

- `query_records` — typed queries over the full load order's plugin records
- `resolve_form` — FormID / EditorID lookup with mod provenance
- `player_history` — what the player did, parsed from saves
- `world_state` — faction standings, quest states, settlement status
- `load_order` — what's installed, versions, conflicts

Agents ground every generation in reality through these tools instead of hallucinating
FormIDs. The server wraps the same Mutagen-based extraction as layer 4.

## 3. Generation (agent layer)

Specialized agents per content type — quest designer, dialogue writer, item balancer,
lore keeper, encounter director — orchestrated with fan-out (the HOI4 Marcus Hale
scuba-diver pattern). Two hard rules:

1. **Agents emit mod specs, never engine bytes.** Output is declarative YAML/JSON
   describing records, scripts, and assets to produce.
2. **Agents must cite** — every reference to an existing form comes from a layer-2 query,
   carried into the spec with provenance.

## 4. Build system (deterministic layer)

`modspec → mod`. A .NET pipeline:

| Input | Tool | Output |
|-------|------|--------|
| record declarations | **Mutagen** | .esp / .esl plugin |
| Papyrus source | `PapyrusCompiler.exe` | .pex compiled scripts |
| texture jobs | diffusion → `texconv` | .dds |
| audio jobs | TTS → xwm encode | voice .xwm + .lip |
| loose files | `Archive2.exe` | .ba2 archives |
| everything | manifest writer | provenance manifest |

No AI inside this layer. Same spec in, same mod out. Every artifact traceable to the
spec line that requested it.

## 5. Validation & feedback

Gates, in order of cost:

1. **Spec lint** — schema-valid, references resolve, no orphan forms
2. **Build gate** — plugin round-trips through Mutagen clean; Papyrus compiles
3. **Consistency gate** — xEdit-style checks: no deleted navmeshes, no wild edits,
   masters sorted, no ITM/UDR records
4. **Smoke test** — game launches with the mod, reaches main menu, loads a test save
   (F4SE-side heartbeat)
5. **Telemetry return** — post-session save parse: did the player encounter the content?
   finish it? break it? Results flow back to layer 1.

## Cross-game strategy

Mutagen supports Fallout 4, Skyrim SE, and Starfield. The mod spec is game-flavored but
shares a core schema; backends live in layer 4. Nothing in layers 1–3 should hard-code
Creation-Engine-specific byte knowledge.
