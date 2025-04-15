using System.Text.Json;

namespace rogue.Data;

public class GameOverStatSaver {
    public List<Statistics> ?gameOverStatisticsList;

    public GameOverStatSaver() {

        string savePath = FindCorrectFilePath();
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameOverStatisticsList = JsonSerializer.Deserialize<List<Statistics>>(json);
            if (gameOverStatisticsList == null) {
                gameOverStatisticsList = [];
            }
        } else
        {
            gameOverStatisticsList = [];
        }
    }

    public void AddRunStatistics(Statistics stat) {
        gameOverStatisticsList.Add(stat);
    }

    public void SaveToJSONBeforeTerminating()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string savePath = FindCorrectFilePath();
        string json = JsonSerializer.Serialize(gameOverStatisticsList, options);
        
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
        File.WriteAllText(savePath, json);
    }

    public string FindCorrectFilePath()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\.."));
        string path = Path.Combine(projectRoot, "saves", "save.json");
        return path;
    }
}