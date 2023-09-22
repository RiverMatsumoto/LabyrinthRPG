using System.Collections.Generic;
using System.Linq;
using Godot;

public class Party
{
    public List<Character> Characters => FrontRow.Concat(BackRow).ToList();
    private List<Character> FrontRow { get; set; }
    private List<Character> BackRow { get; set; }

    public Party()
    {
        FrontRow = new List<Character>();
        BackRow = new List<Character>();
    }
    
    public void AddToFrontRow(Character character)
    {
        if (FrontRow.Count < 3)
            FrontRow.Add(character);
        else
            GD.PrintErr("Tried to add Character to front row. Front row is full");
    }
    
    public void AddToBackRow(Character character)
    {
        if (BackRow.Count < 3)
            BackRow.Add(character);
        else
            GD.PrintErr("Tried to add Character to back row. Back row is full");
    }
}