using System.Collections.Generic;
using System.Linq;
using Godot;

public class Party
{
    public List<CharacterDef> Characters => FrontRow.Concat(BackRow).ToList();
    private List<CharacterDef> FrontRow { get; set; }
    private List<CharacterDef> BackRow { get; set; }

    public Party()
    {
        FrontRow = new List<CharacterDef>();
        BackRow = new List<CharacterDef>();
    }

    public void AddToFrontRow(CharacterDef character)
    {
        if (FrontRow.Count < 3)
            FrontRow.Add(character);
        else
            GD.PrintErr("Tried to add Character to front row. Front row is full");
    }

    public void AddToBackRow(CharacterDef character)
    {
        if (BackRow.Count < 3)
            BackRow.Add(character);
        else
            GD.PrintErr("Tried to add Character to back row. Back row is full");
    }
}
