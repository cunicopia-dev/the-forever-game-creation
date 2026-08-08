# Toolchain

What we build on, sorted by how headless it is.

## Fully programmatic

| Tool | Role | Interface |
|------|------|-----------|
| **Mutagen** | read/write .esp/.esm/.esl as strongly-typed C# objects; FO4 + Skyrim + Starfield | .NET library |
| **PapyrusCompiler.exe** | compile Papyrus scripts (ships with CK) | CLI |
| **texconv** | DDS texture conversion | CLI |
| **Archive2.exe** | pack loose files into BA2 (ships with CK) | CLI |
| **BAE** | extract vanilla BA2 archives | CLI-able |
| **Save parser** | player telemetry from .fos saves (documented format; in-house prior art: EU5/HOI4 parsers) | to build |
| **xVASynth-class TTS** | voice lines in game voices | local API |

## Scriptable with effort

| Tool | Role | Notes |
|------|------|-------|
| **Blender + NIF addons** | procedural meshes | full python scripting; heavy pipeline |
| **CK command line** | precombines/previs, facegen export | batchable-with-pain; the tier-4 unlock |
| **xLODGen / FO4LODGen** | LOD generation | semi-CLI |

## GUI-bound (human or automation-hostile)

| Tool | Role | Why it matters |
|------|------|----------------|
| **Creation Kit** | navmesh, cell layout, previs | the wall between tier 3 and tier 4 |
| **FO4Edit** | interactive conflict inspection | we replicate its *checks* programmatically (its scripting engine is Pascal — we'd rather use Mutagen) |

## Local environment (as of Aug 2026)

- Fallout 4 **1.10.163** (downgraded, pre-next-gen — the moddable target), X:\SteamLibrary
- F4SE 1.10.163 installed, with `src/f4se` plugin source available
- FO4Edit 4.1.5f in the game folder
- Creation Kit installed via Steam (1.11.137 — **needs downgrade to match**, `Downgrade Creation Kit Only.bat`)
- Vortex as mod manager; Buffout 4 crash logging active
- `CreationKitCustom.ini` (multi-master) and `Fallout4Custom.ini` (loose-file invalidation) configured
