using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EnemyRegistry : Resource
{
    [Export] private Dictionary<string, BattlerBase> _enemies;

    // private void LoadEnemies()
    // {
    //     string enemiesPath = "res://resources/enemies/";
    //     var dir = DirAccess.Open(enemiesPath);

    //     if (dir == null)
    //     {
    //         GD.PrintErr($"Failed to open enemies directory: {enemiesPath}");
    //         return;
    //     }

    //     dir.ListDirBegin();
    //     string fileName = dir.GetNext();

    //     while (!string.IsNullOrEmpty(fileName))
    //     {
    //         if (!fileName.StartsWith(".") && fileName.EndsWith(".tres"))
    //         {
    //             string fullPath = $"{enemiesPath}{fileName}";
    //             var enemyResource = (BattlerBase)GD.Load(fullPath);

    //             if (enemyResource != null)
    //             {
    //                 string enemyName = fileName.Replace(".tres", "");
    //                 _enemies[enemyName] = enemyResource;
    //                 GD.Print($"Loaded enemy: {enemyName}");
    //             }
    //             else
    //             {
    //                 GD.PrintErr($"Failed to load enemy: {fullPath}");
    //             }
    //         }
    //         fileName = dir.GetNext();
    //     }

    //     dir.ListDirEnd();
    // }

    public BattlerBase GetEnemy(string name)
    {
        if (_enemies.TryGetValue(name, out BattlerBase enemy))
        {
            return enemy;
        }

        GD.PrintErr($"Enemy '{name}' not found in registry");
        return null;
    }

    public bool HasEnemy(string name)
    {
        return _enemies.ContainsKey(name);
    }

    public string[] GetEnemyNames()
    {
        var names = new string[_enemies.Count];
        _enemies.Keys.CopyTo(names, 0);
        return names;
    }

    public int EnemyCount => _enemies.Count;
}
