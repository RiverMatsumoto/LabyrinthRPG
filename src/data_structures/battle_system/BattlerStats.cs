public class BattlerStats
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Hp { get; set; }
    public int Tp { get; set; }
    public int Str { get; set; }
    public int Tec { get; set; }
    public int Agi { get; set; }
    public int Vit { get; set; }
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
        Luc = luc;
    }

}
