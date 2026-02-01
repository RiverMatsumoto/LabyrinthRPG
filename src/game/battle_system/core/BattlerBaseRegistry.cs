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
            if (party.FrontRowCount < party.MaxMembersRow)
            {
                party.AddToFrontRow(new Battler(battlerBase));
            }
            else if (party.BackRowCount < party.MaxMembersRow)
            {
                party.AddToBackRow(new Battler(battlerBase));
            }
        }
        return party;
    }
#endif
}
