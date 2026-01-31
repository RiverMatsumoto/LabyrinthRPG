using Godot;
using Godot.Collections;

[GlobalClass]
public sealed partial class BattlerBaseRegistry : Resource
{
    [Export] public Array<BattlerBase> BattlerBases { get; private set; }
    [Export] public Array<BattlerBase> DebugParty { get; private set; }

#if DEBUG
    public Party GetDebugParty()
    {
        Party party = new();
        foreach (BattlerBase battlerBase in DebugParty)
        {
            if (party.FrontRowCount < Party.MAX_MEMBERS_ROW)
            {
                party.AddToFrontRow(new Battler(battlerBase));
            }
            if (party.BackRowCount < Party.MAX_MEMBERS_ROW)
            {
                party.AddToBackRow(new Battler(battlerBase));
            }
        }
        return party;
    }
#endif
}
