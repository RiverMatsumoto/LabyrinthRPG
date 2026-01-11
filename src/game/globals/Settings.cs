using Godot;
using System;
using System.IO;
using System.Text.Json;

public class Settings(TextSaveStore jsonSaveStore)
{
    public SettingsData Data { get; set; }
    private readonly string DataPath = Util.UserPath("data/settings.json");
    private readonly TextSaveStore jsonUtil = jsonSaveStore;

    public void LoadSettings()
    {
        try
        {
            var json = jsonUtil.Read(DataPath);
            Data = JsonSerializer.Deserialize<SettingsData>(json)
                   ?? new SettingsData();
        }
        catch
        {
            Data = new SettingsData();
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        // Serialize first so we fail before touching the existing file.
        string json;
        try
        {
            json = JsonSerializer.Serialize(Data);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to serialize settings.", ex);
        }

        // Ensure directory exists.
        var dir = Path.GetDirectoryName(DataPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        // Atomic write pattern:
        // 1) write to temp file in same directory
        // 2) flush to disk
        // 3) replace target (atomic on same filesystem)
        // 4) best-effort cleanup
        string tempPath = DataPath + ".tmp";

        try
        {
            using (var fs = new FileStream(
                       tempPath,
                       FileMode.Create,
                       System.IO.FileAccess.Write,
                       FileShare.None,
                       bufferSize: 4096,
                       options: FileOptions.WriteThrough))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(json);
                sw.Flush();
                fs.Flush(true);
            }

            if (File.Exists(DataPath))
            {
                // Replace is atomic and can create a backup.
                // If you do not want a backup, pass null for the backup path.
                File.Replace(tempPath, DataPath, DataPath + ".bak", ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempPath, DataPath);
            }
        }
        catch (Exception ex)
        {
            // Try to clean up the temp file if something failed.
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch
            {
                // ignore cleanup errors
            }

            throw new IOException($"Failed to save settings to '{DataPath}'.", ex);
        }
    }

    public void LoadDefaultSettings()
    {
        Data = new SettingsData();
        SaveSettings();
    }

}

public class SettingsData
{
    public SettingsData()
    {
        Fps = 60;
        Volume = 1.0f;
        Fullscreen = true;
        // auto primary screen resolution
        Vector2I screenSize = DisplayServer.ScreenGetSize();
        ResolutionWidth = screenSize.X;
        ResolutionHeight = screenSize.Y;
        Language = "en";
    }

    public int Fps { get; set; }
    public float Volume { get; set; }
    public bool Fullscreen { get; set; }
    public int ResolutionWidth { get; set; }
    public int ResolutionHeight { get; set; }
    public string Language { get; set; }
}
