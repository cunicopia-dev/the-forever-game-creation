// Roadmap item 1: prove Mutagen can read the real game data.
// Reads a plugin (default: the live Fallout4.esm), dumps record-type census + samples.
using System.Diagnostics;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;

// lookup mode: MutagenSpike lookup <hexFormId> [pluginPath]
if (args.Length >= 2 && args[0] == "lookup")
{
    var id = Convert.ToUInt32(args[1], 16);
    var path = args.Length > 2 ? args[2] : @"X:\SteamLibrary\steamapps\common\Fallout 4\Data\Fallout4.esm";
    using var lmod = Fallout4Mod.CreateFromBinaryOverlay(path, Fallout4Release.Fallout4);
    var hit = lmod.EnumerateMajorRecords().FirstOrDefault(r => r.FormKey.ID == id);
    if (hit is null) { Console.WriteLine($"No record {id:X6} in {Path.GetFileName(path)}"); return 1; }
    Console.WriteLine($"{hit.FormKey}  {hit.GetType().Name.Replace("BinaryOverlay", "")}  EditorID: {hit.EditorID}");
    if (hit is Mutagen.Bethesda.Fallout4.ILeveledItemGetter lvli)
    {
        Console.WriteLine($"Flags: {lvli.Flags}  ChanceNone: {lvli.ChanceNone}  Entries: {lvli.Entries?.Count ?? 0}");
        foreach (var e in lvli.Entries ?? [])
            Console.WriteLine($"  level {e.Data?.Level,3}  count {e.Data?.Count,3}  -> {e.Data?.Reference.FormKey}");
    }
    return 0;
}

// overrides mode: MutagenSpike overrides <hexFormId> — list every plugin in the live
// load order that carries a version of the record, in load order (last = winner).
if (args.Length >= 2 && args[0] == "overrides")
{
    var id = Convert.ToUInt32(args[1], 16);
    var fk = new FormKey(ModKey.FromNameAndExtension("Fallout4.esm"), id);
    using var env = Mutagen.Bethesda.Environments.GameEnvironment.Typical
        .Construct(Mutagen.Bethesda.GameRelease.Fallout4);
    Console.WriteLine($"Load order: {env.LoadOrder.Count} entries. Scanning for {fk} ...");
    foreach (var listing in env.LoadOrder.ListedOrder)
    {
        if (listing.Mod is not IFallout4ModGetter m) continue;
        var rec = m.LeveledItems.FirstOrDefault(r => r.FormKey == fk);
        if (rec is null) continue;
        Console.WriteLine($"{m.ModKey.FileName}  ({rec.Entries?.Count ?? 0} entries, ChanceNone {rec.ChanceNone})");
        foreach (var e in rec.Entries ?? [])
            Console.WriteLine($"    level {e.Data?.Level,3} count {e.Data?.Count,3} -> {e.Data?.Reference.FormKey}");
    }
    return 0;
}

var esmPath = args.Length > 0
    ? args[0]
    : @"X:\SteamLibrary\steamapps\common\Fallout 4\Data\Fallout4.esm";

if (!File.Exists(esmPath))
{
    Console.Error.WriteLine($"Not found: {esmPath}");
    return 1;
}

var sw = Stopwatch.StartNew();
using var mod = Fallout4Mod.CreateFromBinaryOverlay(esmPath, Fallout4Release.Fallout4);

var counts = new Dictionary<string, int>();
long total = 0;
foreach (var rec in mod.EnumerateMajorRecords())
{
    var typeName = rec.GetType().Name.Replace("BinaryOverlay", "");
    counts[typeName] = counts.GetValueOrDefault(typeName) + 1;
    total++;
}
sw.Stop();

Console.WriteLine($"== {Path.GetFileName(esmPath)} ==");
Console.WriteLine($"ModKey: {mod.ModKey}  Masters: {mod.ModHeader.MasterReferences.Count}");
Console.WriteLine($"Total major records: {total:N0}  (parsed in {sw.ElapsedMilliseconds:N0} ms)");
Console.WriteLine();
Console.WriteLine("Top 25 record types:");
foreach (var (type, count) in counts.OrderByDescending(kv => kv.Value).Take(25))
    Console.WriteLine($"  {type,-30} {count,8:N0}");

Console.WriteLine();
Console.WriteLine("Sample weapons:");
foreach (var weap in mod.Weapons.Take(8))
    Console.WriteLine($"  {weap.FormKey}  {weap.EditorID,-28} \"{weap.Name?.String}\"");

Console.WriteLine();
Console.WriteLine("Sample quests:");
foreach (var qust in mod.Quests.Take(8))
    Console.WriteLine($"  {qust.FormKey}  {qust.EditorID,-28} \"{qust.Name?.String}\"");

return 0;
