using System.Collections.Generic;
using System.Linq;

public class BattleModel
{
    public List<Battler> playerPartyFrontRow = new();
    public List<Battler> playerPartyBackRow = new();
    public List<Battler> playerPartyFull => (List<Battler>)playerPartyFrontRow.Concat(playerPartyBackRow);
    public List<Battler> enemyParty = new();

    public BattleModel() { }

    public void ResolveTurn() { }
}
