using System.Collections.Generic;
using System.Linq;
using Godot;

public class Party
{
    public IList<Battler> Battlers => FrontRow.Concat(BackRow).ToList();
    private IList<Battler> FrontRow { get; set; } = [];
    private IList<Battler> BackRow { get; set; } = [];

    public void AddToFrontRow(Battler battler)
    {
        if (FrontRow.Count < 3)
            FrontRow.Add(battler);
        else
            GD.PrintErr("Tried to add Character to front row. Front row is full");
    }

    public void AddToBackRow(Battler battler)
    {
        if (BackRow.Count < 3)
            BackRow.Add(battler);
        else
            GD.PrintErr("Tried to add Character to back row. Back row is full");
    }
}
