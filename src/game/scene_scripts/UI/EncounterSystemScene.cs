using Godot;

public partial class EncounterSystemScene : CanvasLayer
{
    [Export] private PlayerMovement playerMovement;
    [Signal] public delegate void EncounterTriggeredEventHandler(EncounterData encounterData);

    public float EncounterBar
    {
        get => EncounterBar;
        private set
        {
            if (EncounterBar + value > 1.0f)
            {
                EncounterBar = 0f;
                // TODO implement an encounter provider that tracks current
                // floor and selects from a pool of encounters
                EmitSignal(
                    SignalName.EncounterTriggered,
                    new EncounterData
                    {
                        Enemies = new() { "squid_wizard" },
                        Floor = 1
                    }
                );

            }
            else
                EncounterBar = value;
        }
    }

    public override void _Ready()
    {
        playerMovement.OnMoveComplete += OnStep;
    }


    // percentage until encounter
    public void OnStep()
    {
        var rng = new RandomNumberGenerator();
        EncounterBar = rng.RandfRange(0, 0.08f);
    }
}
