using System.IO;
using Godot;
using Newtonsoft.Json;

public partial class Settings : Node
{
    public static Data data { get; set; }
    private static readonly string DataPath = Util.Path("data/settings.json");

    public override void _Ready()
    {
        LoadSettings();
    }

    public static void LoadSettings()
    {
        if (File.Exists(DataPath))
        {
            var sr = new StreamReader(DataPath);
            var jtr = new JsonTextReader(sr);
            data = JsonSerializer.Create().Deserialize<Data>(jtr);
            sr.Close();
        }
        else
            LoadDefaultSettings();
    }

    public static void SaveSettings()
    {
        var sr = new StreamWriter(DataPath);
        var jtr = new JsonTextWriter(sr);
        jtr.Formatting = Formatting.Indented;
        jtr.Indentation = 4;
        JsonSerializer.Create().Serialize(jtr, data);
        sr.Close();
    }

    public static void LoadDefaultSettings()
    {
        data = new Data();
        SaveSettings();
    }
    
    public class Data
    {
        public Data()
        {
            Fps = 60;
        }
        public int Fps { get; set; }
    }
}
