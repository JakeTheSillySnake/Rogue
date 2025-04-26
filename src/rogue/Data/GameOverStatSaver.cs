using Newtonsoft.Json;
using System.Text.Json;
using static rogue.Data.SessionDataSaver;

namespace rogue.Data;

public static class GameOverStatSaver {
    public class GameOverStatisticsJSON
    {
        public List<Statistics>? GameOverStatisticsList { get; private set; }

        public GameOverStatisticsJSON()
        {
            GameOverStatisticsList = [];
        }
        public GameOverStatisticsJSON( List<Statistics> gameOverStatisticsList )
        {
            GameOverStatisticsList = gameOverStatisticsList;
        }
    }

    public static List<Statistics>? GetGameOverStatData()
    {
        GameOverStatisticsJSON? gameOverStatisticsJSON = new GameOverStatisticsJSON();
        List<Statistics> gameOverStatisticsList = new List<Statistics>();
        string savePath = FindCorrectFilePath();
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameOverStatisticsJSON = JsonConvert.DeserializeObject<GameOverStatisticsJSON>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            gameOverStatisticsList = gameOverStatisticsJSON.GameOverStatisticsList;
            if (gameOverStatisticsList == null)
            {
                gameOverStatisticsList = [];
            }
        }
        else
        {
            gameOverStatisticsList = new List<Statistics>();
        }
        return gameOverStatisticsList;
    }

    public static void AddRunStatistics(List<Statistics> gameOverStatisticsList, Statistics stat) {
        gameOverStatisticsList.Add(stat);
    }

  public static void LoadData(List<Statistics> gameOverStatisticsList) {
    GameOverStatisticsJSON? gameOverStatisticsJSON = new GameOverStatisticsJSON(gameOverStatisticsList);
    string savePath = FindCorrectFilePath();
    string json = JsonConvert.SerializeObject(gameOverStatisticsJSON, new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All
    });
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
    File.WriteAllText(savePath, json);
  }

  public static string FindCorrectFilePath() {
    string projectRoot =
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\.."));
    string path = Path.Combine(projectRoot, "saves", "GameoverData.json");
    return path;
  }
}