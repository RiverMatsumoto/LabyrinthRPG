using System.Collections.Generic;
using System.Linq;
using Godot;

public class Party
{
    public List<Battler> Battlers => FrontRow.Concat(BackRow).ToList();
    private List<Battler> FrontRow { get; set; } = [];
    private List<Battler> BackRow { get; set; } = [];

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
