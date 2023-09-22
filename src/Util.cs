using Godot;

public static class Util
{
    public static string Path(string path) => Godot.ProjectSettings.GlobalizePath("res://" + path);
}