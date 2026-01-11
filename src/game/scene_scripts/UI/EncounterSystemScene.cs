using Godot;

public partial class EncounterSystemScene : CanvasLayer
{
    [Signal] public delegate void EncounterTriggeredEventHandler(EncounterData encounterData);
    [Export] private PlayerMovement playerMovement;
    [Export] private EncounterData encounterData1;
    [Export] private ProgressBar progressBar;
    [Export] public float EncounterSpeed = 0.1f;

    private float _encounterBar;
    private readonly RandomNumberGenerator _rng = new();

    public float EncounterBar
    {
        get => _encounterBar;
        private set
        {
            var v = Mathf.Clamp(value, 0f, 1f);
            _encounterBar = v;

            if (_encounterBar >= 1f)
            {
                _encounterBar = 0f;
                EmitSignal(SignalName.EncounterTriggered, encounterData1);
            }

            if (progressBar != null)
                progressBar.Value = _encounterBar; // if your bar expects 0..1
        }
    }

    public override void _Ready()
    {
        _rng.Randomize();
    }

    public void OnStep()
    {
        EncounterBar += _rng.RandfRange(0f, EncounterSpeed);
    }
}
