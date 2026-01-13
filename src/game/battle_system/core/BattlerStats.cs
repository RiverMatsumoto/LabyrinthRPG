using System.Collections.Generic;
using System.Linq;

public class BattlerStats
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Atk => AtkBonus.Values.Sum();
    public int Def => DefBonus.Values.Sum();
    public Dictionary<string, int> AtkBonus = new();
    public Dictionary<string, int> DefBonus = new();
    public int Hp { get; set; }
    public int Tp { get; set; }
    public int Str { get; set; }
    public int Tec { get; set; }
    public int Agi { get; set; }
    public int Vit { get; set; }
    public int Wis { get; set; }
    public int Luc { get; set; }

    public BattlerStats()
    {
        Name = "Debug";
        Description = "No description";
        Level = 1;
        Experience = 0;
        Hp = 100;
        Tp = 50;
        Str = 10;
        Tec = 10;
        Agi = 10;
        Vit = 10;
        Wis = 10;
        Luc = 10;
    }

    public BattlerStats(
        string name,
        string description,
        int level,
        int experience,
        int hp,
        int tp,
        int str,
        int tec,
        int agi,
        int vit,
        int wis,
        int luc)
    {
        Name = name;
        Description = description;
        Level = level;
        Experience = experience;
        Hp = hp;
        Tp = tp;
        Str = str;
        Tec = tec;
        Agi = agi;
        Vit = vit;
        Wis = wis;
        Luc = luc;
    }

    public BattlerStats(BattlerBase bb)
    {
        Name = bb.Name;
        Description = bb.Description;
        Level = bb.Level;
        Experience = bb.Experience;
        Hp = bb.Hp;
        Tp = bb.Tp;
        Str = bb.Str;
        Tec = bb.Tec;
        Agi = bb.Agi;
        Vit = bb.Vit;
        Wis = bb.Wis;
        Luc = bb.Luc;
    }


}
