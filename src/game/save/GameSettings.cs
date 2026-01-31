using Godot;

// later can write a function that sets smarter defaults
public sealed class GameSettings
{
    public float Fps { get; set; }
    public float Volume { get; set; }
    public bool Fullscreen { get; set; }
    public int ResolutionWidth { get; set; }
    public int ResolutionHeight { get; set; }
    public string Language { get; set; }
    public BattlePlaybackOptions PlaybackOptions { get; set; }

    public static GameSettings GetBaseSettings()
    {
        return new GameSettings
        {
            Fps = DisplayServer.ScreenGetRefreshRate(),
            Volume = 0.8f,
            Fullscreen = true,
            ResolutionWidth = DisplayServer.ScreenGetSize().X,
            ResolutionHeight = DisplayServer.ScreenGetSize().Y,
            Language = "en",
            PlaybackOptions = new BattlePlaybackOptions()
        };
    }
}
