#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Length < 2)
{
    Console.Error.WriteLine("usage: EffectSchemaGen <path/to/GameOrCore.dll> <out/schema.json> [TargetEnumName=TargetRule]");
    return 2;
}

var dllPath = Path.GetFullPath(args[0]);
var outPath = Path.GetFullPath(args[1]);
var targetEnumName = args.Length >= 3 ? args[2] : "TargetRule";

var asmDir = Path.GetDirectoryName(dllPath)!;

var alc = new AssemblyLoadContext("schema-gen", isCollectible: true);
alc.Resolving += (_, name) =>
{
    var p = Path.Combine(asmDir, name.Name + ".dll");
    return File.Exists(p) ? alc.LoadFromAssemblyPath(p) : null;
};

var asm = alc.LoadFromAssemblyPath(dllPath);

// Load sibling assemblies so enums/types referenced from other dlls are discoverable.
foreach (var p in Directory.EnumerateFiles(asmDir, "*.dll"))
{
    var full = Path.GetFullPath(p);
    if (string.Equals(full, dllPath, StringComparison.OrdinalIgnoreCase)) continue;

    try { alc.LoadFromAssemblyPath(full); }
    catch { /* ignore load failures; we only need what loads */ }
}

var ieffect = FindType(asm, "IEffect") ?? throw new Exception("IEffect not found in loaded assembly.");
var effectIdAttr = FindType(asm, "EffectIdAttribute"); // optional

var effects = asm.GetTypes()
    .Where(t => t is { IsAbstract: false, IsInterface: false } && ieffect.IsAssignableFrom(t))
    .OrderBy(t => t.FullName, StringComparer.Ordinal)
    .ToList();

var schema = new RootSchema
{
    target_rule = FindEnumNames(alc, targetEnumName),
    effects = effects.ToDictionary(
        t => GetEffectId(t, effectIdAttr) ?? ToSnakeCase(t.Name.Replace("Effect", "")),
        t => BuildEffectSchema(t),
        StringComparer.Ordinal
    )
};

Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
File.WriteAllText(outPath, JsonSerializer.Serialize(schema, new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
}));

Console.WriteLine($"wrote: {outPath}");
return 0;

// ---------------- helpers ----------------

static Type? FindType(Assembly asm, string simpleName)
    => asm.GetTypes().FirstOrDefault(t => t.Name == simpleName);

// static string[]? FindEnumNames(Assembly asm, string enumName)
// {
//     var t = asm.GetTypes().FirstOrDefault(x => x.IsEnum && x.Name == enumName);
//     return t is null ? null : Enum.GetNames(t);
// }

static string[]? FindEnumNames(AssemblyLoadContext alc, string enumName)
{
    foreach (var a in alc.Assemblies)
    {
        Type? t = null;
        try
        {
            t = a.GetTypes().FirstOrDefault(x => x.IsEnum && x.Name == enumName);
        }
        catch (ReflectionTypeLoadException e)
        {
            t = e.Types.Where(x => x is not null)
                       .Cast<Type>()
                       .FirstOrDefault(x => x.IsEnum && x.Name == enumName);
        }

        if (t is not null)
            return Enum.GetNames(t);
    }
    return null;
}

static string? GetEffectId(Type effectType, Type? effectIdAttr)
{
    if (effectIdAttr is null) return null;
    foreach (var a in effectType.GetCustomAttributes(inherit: false))
    {
        if (a.GetType() != effectIdAttr) continue;

        // supports: public string Id { get; }
        var prop = effectIdAttr.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        if (prop?.PropertyType == typeof(string))
            return (string?)prop.GetValue(a);
    }
    return null;
}

static EffectSchema BuildEffectSchema(Type t)
{
    var ctor = PickBestCtor(t);
    if (ctor is null) return new EffectSchema { fields = new List<FieldSchema>() };

    var fields = ctor.GetParameters().Select(p =>
    {
        var pt = Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType;
        var (kind, values) = Classify(pt);

        object? def = null;
        if (p.HasDefaultValue) def = NormalizeDefault(p.DefaultValue, pt);

        return new FieldSchema
        {
            key = ToSnakeCase(p.Name ?? "arg"),
            kind = kind,
            values = values,
            @default = def
        };
    }).ToList();

    return new EffectSchema { fields = fields };
}

static ConstructorInfo? PickBestCtor(Type t)
    => t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
        .OrderByDescending(c => c.GetParameters().Length)
        .FirstOrDefault();

static (string kind, string[]? values) Classify(Type t)
{
    if (t.IsEnum) return ("enum", Enum.GetNames(t));
    if (t == typeof(bool)) return ("bool", null);
    if (t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte)) return ("int", null);
    if (t == typeof(float) || t == typeof(double) || t == typeof(decimal)) return ("float", null);
    return ("string", null);
}

static object? NormalizeDefault(object? v, Type t)
{
    if (v is null) return null;
    if (t == typeof(float)) return Convert.ToSingle(v, CultureInfo.InvariantCulture);
    if (t == typeof(double)) return Convert.ToDouble(v, CultureInfo.InvariantCulture);
    if (t == typeof(decimal)) return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
    if (t.IsEnum) return v.ToString(); // store enum name
    return v;
}

static string ToSnakeCase(string s)
{
    if (string.IsNullOrEmpty(s)) return s;
    var outChars = new List<char>(s.Length + 8);
    for (int i = 0; i < s.Length; i++)
    {
        var c = s[i];
        if (char.IsUpper(c))
        {
            if (i > 0) outChars.Add('_');
            outChars.Add(char.ToLowerInvariant(c));
        }
        else outChars.Add(c);
    }
    return new string(outChars.ToArray());
}

// ---------------- schema DTOs ----------------

sealed class RootSchema
{
    public string[]? target_rule { get; set; } // dropdown values
    public Dictionary<string, EffectSchema> effects { get; set; } = new(StringComparer.Ordinal);
}

sealed class EffectSchema
{
    public List<FieldSchema> fields { get; set; } = new();
}

sealed class FieldSchema
{
    public string key { get; set; } = "";
    public string kind { get; set; } = "";      // "enum"|"bool"|"int"|"float"|"string"
    public string[]? values { get; set; }       // for enum
    public object? @default { get; set; }       // optional
}
