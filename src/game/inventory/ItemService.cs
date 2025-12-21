using System.Linq;

public sealed class ItemService
{
    public UseResult TryUseFromInventory(
        Inventory inv,
        ItemDef item,
        string actionId,
        UseRequest req)
    {
        if (inv.GetCount(item) <= 0)
            return UseResult.Fail("Item not in inventory.");

        var use = item.Get<UseModule>();
        if (use is null)
            return UseResult.Fail("Item has no use actions.");

        var action = use.Actions.FirstOrDefault(a => a.ActionId == actionId);
        if (action is null)
            return UseResult.Fail("Unknown action.");

        var result = action.TryExecute(req);
        if (result.Error != null) return result;

        if (result.Consumed)
        {
            if (!inv.TryRemove(item, out var err))
                return UseResult.Fail(err ?? "Failed to consume item.");
        }
        return result;
    }
}
