# Capability matrix

Every mod type, sorted by automatability tier. Tiers 1–3 are the **fully-programmatic
zone** — no Creation Kit GUI, no navmesh, no precombines — and cover roughly 80% of mod
categories, including all the narrative-bearing ones.

## Tier 1 — Pure record edits (Mutagen only)

| Mod type | Notes |
|----------|-------|
| Balance / gameplay overhauls | weapon & armor stats, perks, GMSTs, XP curves |
| Loot, economy, leveled lists | leveled-list *injection* distributes new items with zero cell edits |
| Crafting recipes | COBJ records, workbench categories |
| New items from vanilla assets | material swaps (MSWP) × generated stats × legendary effects = unlimited variety |
| NPCs & factions | new NPCs, relationships, merchants, spawns, encounter zones. *Caveat: human facegen — use face-template records or non-human NPCs* |
| Compatibility & bug patches | Synthesis-model whole-load-order patchers; machines beat humans here |
| Weather, lighting, ambience | weather systems, imagespaces, region sounds — all records |

## Tier 2 — Records + Papyrus (mechanics)

| Mod type | Notes |
|----------|-------|
| Gameplay systems | survival, needs, injuries, timers, random events |
| Quests | radiant-style against existing locations = **no navmesh problem**; multi-stage stories in existing spaces |
| Companions | affinity, commentary, own quests (non-human sidesteps facegen) |
| Settlement / workshop content | buildables from vanilla meshes, attack events |
| MCM config menus | generated mods ship user-tunable |

## Tier 3 — Records + Papyrus + generated media (narrative)

| Mod type | Notes |
|----------|-------|
| Terminals, holotapes, notes, books | pure text records — environmental storytelling is a text-generation task with FormID plumbing |
| Voiced dialogue | TTS → WAV/XWM + LIP → BA2 |
| **Radio stations** | quest + sound records + generated audio. A DJ who reports on *your* save = flagship demo |
| Music & soundscapes | file replacement + sound descriptors |
| Texture content | retextures, posters, graffiti, magazine covers via diffusion → texconv |

## Tier 4 — The GUI wall (run phase, years 2–3)

| Mod type | Blocker |
|----------|---------|
| New interior cells / dungeons | navmesh generation (CK); precombines/previs for performance |
| Worldspace edits | moving/deleting precombined refs breaks previs → FPS death |
| Custom meshes | Blender python + NIF pipeline — scriptable but heavy |
| Animation | hardest asset class; defer |
| F4SE native plugins | C++ track; programmable but separate toolchain |

CK does expose command-line switches for precombine/previs generation — batchable with
pain. This is the hardest engineering problem in the roadmap and the moat if solved.
