# The mod spec

The declarative intermediate representation between AI intent and engine bytes — the
load-bearing artifact of the whole system. Agents (layer 3) emit specs; the build system
(layer 4) compiles them. Games change, models change; the IR survives.

## Design rules

1. **Declarative.** A spec describes *what exists*, never *how to build it*.
2. **Grounded.** Every reference to an existing form carries provenance from a layer-2
   query (`master`, `formid`/`editorid`, and the query that found it).
3. **Game-flavored, core-shared.** `game: fallout4` selects the backend; the record
   vocabulary is per-game, the envelope/asset/script/validation schema is shared.
4. **Deterministic.** Same spec + same load order + same seed = byte-identical mod.
5. **Self-describing canon.** Specs declare the lore facts they introduce, which are
   written back into the world model on deploy.

## Envelope (v0 draft)

```yaml
modspec: 0.1
game: fallout4
meta:
  name: <mod name>
  id: <stable slug>
  version: <semver>
  canon: []            # lore facts this mod adds to the world model
requires: []           # masters + soft dependencies, with provenance
records: []            # typed record declarations (per-game vocabulary)
scripts: []            # papyrus sources to compile + attach
assets: []             # texture/audio/mesh jobs for the asset pipeline
distribution: []       # leveled-list injections, vendor adds, placement rules
validation: {}         # per-mod overrides for the gate suite
```

See [examples/](examples/) — the unique-weapon spec is the v0 reference case.
