using System.IO;

public sealed class FileSaveStore : ISaveStore
{
    public Save Read(string path)
    {
        if (!File.Exists(path))
            return new Save();

        byte[] data = File.ReadAllBytes(path);
        return SaveSerializer.Deserialize(data);
    }

    public void Write(string path, Save save)
    {
        byte[] data = SaveSerializer.Serialize(save);

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllBytes(path, data);
    }
}
