using System;
using Godot;

[Serializable]
public class Save
{
    // Temporary save data for debugging
    public float PlayerHealth { get; set; } = 100f;
    public int PlayerLevel { get; set; } = 1;
    public int ExperiencePoints { get; set; } = 0;
    public Vector3I PlayerPosition { get; set; } = Vector3I.Zero;

}
