using Godot;

public static class Util
{
    public static string ReadonlyPath(string path) => Godot.ProjectSettings.GlobalizePath("res://" + path);
    public static string UserPath(string path) => Godot.ProjectSettings.GlobalizePath("user://" + path);
}
