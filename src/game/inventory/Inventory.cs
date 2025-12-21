using System.Collections.Generic;
using System.Linq;


public sealed class Inventory
{
    private readonly List<ItemDef> _items = new();
    public int MaxItems { get; }

    public Inventory(int maxItems = 60)
    {
        MaxItems = maxItems;
    }

    public IReadOnlyList<ItemDef> Items => _items;

    public int GetCount(ItemDef item) => _items.Count(i => i.Id == item.Id);

    public bool CanAdd(ItemDef item)
    {
        // Each item consumes one slot; cannot exceed MaxItems.
        return _items.Count < MaxItems;
    }

    public bool TryAdd(ItemDef item, out string error)
    {
        error = null;
        if (_items.Count >= MaxItems)
        {
            error = "Inventory is full.";
            return false;
        }

        _items.Add(item);
        return true;
    }

    public bool TryRemove(ItemDef item, out string error)
    {
        error = null;
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index == -1)
        {
            error = "Item not found.";
            return false;
        }

        _items.RemoveAt(index);
        return true;
    }

    public IEnumerable<ItemDef> EnumerateItems() => _items.AsReadOnly();
}


public sealed class EquipmentSet
{
    private readonly Dictionary<EquipSlot, ItemDef> _equipped = new();

    public bool TryGet(EquipSlot slot, out ItemDef item)
    {
        if (_equipped.TryGetValue(slot, out var i)) { item = i; return true; }
        item = null; return false;
    }

    public bool CanEquip(ItemDef item, out string error)
    {
        error = null;
        var equip = item.Get<EquipModule>();
        if (equip is null) { error = "Item is not equipable."; return false; }
        return true;
    }

    public bool TryEquip(ItemDef item, out string error)
    {
        if (!CanEquip(item, out error)) return false;
        var equip = item.Get<EquipModule>()!;
        _equipped[equip.Slot] = item;
        return true;
    }

    public bool Unequip(EquipSlot slot) => _equipped.Remove(slot);

    public IEnumerable<StatModifier> GetAllStatModifiers()
    {
        foreach (var item in _equipped.Values)
        {
            var mods = item.Get<StatModifiersModule>();
            if (mods is null) continue;
            foreach (var m in mods.Enumerate())
                yield return m;
        }
    }
}
