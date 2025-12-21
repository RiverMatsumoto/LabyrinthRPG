using System;
using System.Collections.Generic;

public interface IActionLibrary
{
    ActionDef Get(string id);
}

public sealed class ActionLibrary(Dictionary<string, ActionDef> defs) : IActionLibrary
{
    private readonly Dictionary<string, ActionDef> _defs = defs;

    public ActionDef Get(string id) => _defs.TryGetValue(id, out var def) ? def : throw new InvalidOperationException($"Unknown action id: {id}");
}
