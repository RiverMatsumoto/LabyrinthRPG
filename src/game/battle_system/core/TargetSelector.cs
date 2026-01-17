
using System.Collections.Generic;
using System.Linq;

public class TargetSelector
{
    public static List<Battler> GetValidTargets(Targeting rule, Battler source, BattleModel model)
    {
        // one suggestion is to use a selection window pattern
        // where the selector/cursor behavior changes
        // based on the targeting rule, for example:
        // Targeting.RowEnemies should have a window that can only select
        // the entire row at a time, not necessarily one enemy within a row
        return rule switch
        {
            Targeting.SingleEnemy => model.enemyParty.ToList(),
            Targeting.SingleAlly => model.playerParty.ToList(),
            Targeting.AllEnemies => model.enemyParty.ToList(),
            Targeting.AllAllies => model.playerParty.ToList(),
            // Targeting.RowEnemies => GetEnemyRow(model, source),
            // Targeting.RowAllies => GetAllyRow(model, source),
            Targeting.Self => new List<Battler> { source },
            _ => new List<Battler>()
        };
    }
}
