using System.Collections.Generic;
using System.Linq;
using Godot;

public class Party
{
    public List<CharacterBase> Characters => FrontRow.Concat(BackRow).ToList();
    private List<CharacterBase> FrontRow { get; set; }
    private List<CharacterBase> BackRow { get; set; }

    public Party()
    {
        FrontRow = new List<CharacterBase>();
        BackRow = new List<CharacterBase>();
    }

    public void AddToFrontRow(CharacterBase character)
    {
        if (FrontRow.Count < 3)
            FrontRow.Add(character);
        else
            GD.PrintErr("Tried to add Character to front row. Front row is full");
    }

    public void AddToBackRow(CharacterBase character)
    {
        if (BackRow.Count < 3)
            BackRow.Add(character);
        else
            GD.PrintErr("Tried to add Character to back row. Back row is full");
    }
}
