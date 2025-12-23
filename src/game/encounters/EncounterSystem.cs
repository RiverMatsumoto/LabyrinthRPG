using System;
using System.Collections.Generic;
using Godot;

public sealed class EncounterSystem
{
    public event Action<EncounterData> EncounterTriggered;
    public float EncounterBar
    {
        get => EncounterBar;
        private set
        {
            if (EncounterBar + value > 1.0f)
            {
                EncounterBar = 0f;
                EncounterTriggered.Invoke(new EncounterData(
                    new List<string>(["squid_wizard"]),
                    1
                ));
            }
            else
                EncounterBar = value;
        }
    }


    // percentage until encounter
    public void OnStep()
    {
        var rng = new RandomNumberGenerator();
        EncounterBar = rng.RandfRange(0, 0.2f);
    }

    public void StartEncounter()
    {

    }
}

// temporary data
public sealed record EncounterData(
    IReadOnlyList<string> Enemies,
    int Floor
);
