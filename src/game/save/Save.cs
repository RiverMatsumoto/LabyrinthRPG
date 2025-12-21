using System;

[Serializable]
public class Save
{
    // Temporary save data for debugging
    public int Fps { get; set; } = 60;
    public float PlayerHealth { get; set; } = 100f;
    public int PlayerLevel { get; set; } = 1;
    public int ExperiencePoints { get; set; } = 0;
    public float[] PlayerPosition { get; set; } = new float[3] { 0f, 0f, 0f };

}
