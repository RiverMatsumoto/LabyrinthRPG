public class Battler
{
    public BattlerStats Stats;
    public bool IsAlive => Stats.Hp > 0;

    public Battler()
    {
        Stats = new BattlerStats();
    }

    public Battler(BattlerStats stats)
    {
        Stats = stats;
    }
}
