
public interface ISaveStore
{
    Save Read(string path);
    void Write(string path, Save save);
}
