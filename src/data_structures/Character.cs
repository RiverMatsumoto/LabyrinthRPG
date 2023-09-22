public class Character
{
    public string Name { get; set; }
    public string Class { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Hp { get; set; }
    public int Tp { get; set; }
    public int Strength { get; set; }

    public Character()
    {
        Name = "Default";
        Class = "Default";
        Level = 1;
        Experience = 0;
        Hp = 10;
        Tp = 10;
        Strength = 1;
    }
}
