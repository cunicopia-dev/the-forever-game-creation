# Architecture

Six layers (0–5), arranged as a compiler stack where the source language is *intent* and
the target is a load order. AI lives in layers 1–3. Layers 0, 4, and 5 are deterministic
engineering.

The separation of concerns to defend at all costs:

> The **world model** describes *truth*. The **agent layer** describes *intent*. The
> **mod spec** describes *desired change*. The **compiler** describes *implementation*.

Do not let these collapse together. A quest agent never says "create QUST FormID 0x800123,
attach script, set stage 20" — it says "an investigation quest instigated by the mayor,
available because the player spared this NPC three years ago, with these three outcomes."
Deterministic machinery figures out the Creation Engine horror show required to manifest
that intent.

## 0. Provenance ledger (event layer)

Append-only, first-class — not an afterthought bolted to the KG (though kgrdbms's
replayable event log is the in-house prior art). Every generation records:

```
generation_id, parent_generation
input_world_state (KG snapshot ref)
player_events_considered (save telemetry refs)
agent_decisions (which proposals, which chosen, why)
intent_ir + change_ir versions
compiler_version, artifact_hashes
deployment record, resulting_save_states
```

This buys **causal archaeology**: "Why does this NPC believe the player destroyed Outpost
Zeta?" → "Generation 481 introduced the belief because save 229 showed NPC X dead and
quest Y complete; generation 492 reinforced it through dialogue Z."

**The immutability rule:** a generation becomes *immutable the moment any player save has
observed it*. Removing an esp a save has referenced corrupts that save (orphaned script
instances, missing forms). Therefore:

- **Rollback before observation** = deletion. Clean, cheap, encouraged.
- **Rollback after observation** = a *compensating generation* — a retcon, authored
  forward, recorded in the ledger like anything else. The world cannot un-happen events;
  it can only respond. If generation 834 turns Diamond City into a radioactive pumpkin,
  generation 835 is the cleanup crew and the NPCs who talk about that one horrible night.

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

**The graph is not "what exists in Fallout 4."** It is *what exists in this player's
canonical Commonwealth*: `G(base game + DLC + installed mods + current save + generated
history)`. That graph is the game from the intelligence's perspective — the ESPs are
serialization, the save is telemetry, the KG is the semantic world.

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

1. **Agents emit Intent IR, never engine bytes.** Output is declarative YAML/JSON at the
   *intent* level — premises, prerequisites, outcomes, semantic references
   (`npc.raider_lieutenant_001`), never FormIDs or record layouts.
2. **Agents must cite** — every reference to an existing entity comes from a layer-2 query,
   carried into the spec with provenance.
3. **The world reacts selectively.** Agents *propose* consequences; a planner *chooses* a
   few. The world shouldn't react to everything — restraint is what makes reactions land.

## 4. Build system (deterministic layer)

The mod spec is a **lowering stack**, not a single format — HIR → MIR → LIR, like a real
compiler:

```
Intent IR   (agent output: premises, outcomes, semantic refs)
    │  elaboration — deterministic planning: resolve semantic refs via layer 2,
    │  select templates, allocate editor IDs
    ▼
Change IR   (record-level declarations, resolved masters/forms)
    │  backend compilation
    ▼
Engine bytes (esp/esl + pex + dds + ba2)
```

Creation Engine is backend #1. The abstract system is `world ontology → state extractor →
content IR → backend compiler`; a future engine is `ModSpec → JSON + Lua + asset bundle`,
Minecraft is `ModSpec → datapack + resource pack`. The IR outlives models and engines.

The Change-IR-to-bytes pipeline:

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
