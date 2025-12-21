using System;
using System.IO;

public class TextSaveStore
{
    public string Read(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        try
        {
            var s = File.ReadAllText(path);
            return s;
        }
        catch (FileNotFoundException)
        {
            throw new InvalidOperationException($"Save file not found: {path}");
        }
        catch (UnauthorizedAccessException)
        {
            throw new InvalidOperationException($"Access denied to file: {path}");
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"I/O error while reading file: {path}", ex);
        }
    }

    public void Write(string path, string text)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        if (text == null)
            throw new ArgumentNullException(nameof(text));

        try
        {
            File.WriteAllText(path, text);
        }
        catch (UnauthorizedAccessException)
        {
            throw new InvalidOperationException($"Access denied to file: {path}");
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"I/O error while writing file: {path}", ex);
        }
    }
}
