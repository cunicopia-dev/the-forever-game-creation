// Roadmap item 1: prove Mutagen can read the real game data.
// Reads a plugin (default: the live Fallout4.esm), dumps record-type census + samples.
using System.Diagnostics;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;

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
