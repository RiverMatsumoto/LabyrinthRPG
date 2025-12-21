using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

internal static class SaveSerializer
{
    private const int CurrentVersion = 1;

    private static readonly Encoding Encoding =
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private sealed class SaveEnvelope
    {
        public int Version { get; set; }
        public Save Data { get; set; } = new Save();
    }

    public static byte[] Serialize(Save save)
    {
        var envelope = new SaveEnvelope
        {
            Version = CurrentVersion,
            Data = save
        };

        string json = JsonSerializer.Serialize(envelope, JsonOptions);
        byte[] jsonBytes = Encoding.GetBytes(json);

        return GzipCompress(jsonBytes);
    }

    public static Save Deserialize(byte[] data)
    {
        byte[] jsonBytes = GzipDecompress(data);
        string json = Encoding.GetString(jsonBytes);

        var envelope = JsonSerializer.Deserialize<SaveEnvelope>(json, JsonOptions);
        if (envelope == null || envelope.Data == null)
            return new Save();

        if (envelope.Version > CurrentVersion)
            throw new InvalidDataException(
                $"Unsupported save version {envelope.Version}"
            );

        return envelope.Data;
    }

    private static byte[] GzipCompress(byte[] data)
    {
        using var ms = new MemoryStream();
        using (var gz = new GZipStream(ms, CompressionLevel.Fastest))
        {
            gz.Write(data, 0, data.Length);
        }
        return ms.ToArray();
    }

    private static byte[] GzipDecompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var gz = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gz.CopyTo(output);
        return output.ToArray();
    }
}
