# Roadmap — 2026 → 2030

## Crawl (months) — prove the pipeline

- [ ] Mutagen spike: read Fallout4.esm, dump record stats (de-risks the load-bearing dependency)
- [ ] Mod-spec v0 schema + validator
- [ ] Build system MVP: spec → esp via Mutagen
- [ ] First generated mod, data-only: a unique weapon (vanilla mesh + material swap +
      generated texture + lore + leveled-list injection)
- [ ] `fo4` MCP server v0: query_records / resolve_form over the load order
- [ ] .fos save parser v0: quest states + player stats
- [ ] `fo4` kgrdbms ontology seeded from vanilla records

**Exit criterion:** an agent, using only MCP tools and the spec format, ships a working
item mod into the load order with zero manual steps.

## Walk (year 1) — narrative

- [ ] Papyrus generation + compile gate in the build system
- [ ] Quest specs: radiant-style (kill/clear/fetch against existing locations)
- [ ] TTS voice pipeline (WAV/XWM + LIP + BA2)
- [ ] Terminals / holotapes / notes as first-class spec objects
- [ ] Save telemetry → world model feedback loop closes
- [ ] **Demo: the save-aware radio station.** A DJ who reports on your actual playthrough.
- [ ] **Demo: a voiced quest that references what's in your save.**

**Exit criterion:** the early demonstration of the forever game — play, save, and the
next session's content knows what you did.

## Run (years 2–3) — spaces

- [ ] CK batch automation: navmesh + precombines/previs via command line (the hard problem)
- [ ] Interior cell / dungeon generation
- [ ] Blender procedural mesh pipeline
- [ ] Skyrim SE backend for the mod spec; Starfield evaluation
- [ ] Multi-mod canon: generated content referencing older generated content

## Fly (2030) — the closed loop

- [ ] Continuous operation: session → ingest → generate → validate → deploy, overnight
- [ ] Long-horizon narrative arcs planned across many sessions
- [ ] The world moves while you sleep.
