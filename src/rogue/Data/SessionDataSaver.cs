namespace rogue.Data;

using rogue.Domain;
using rogue.Domain.LevelMap;
using rogue.Domain.Enemies;
using rogue.Domain.Items;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class SessionDataSaver {
  public class SessionDataJSON {
    public List<List<int>> FieldData { get; set; }
    public List<Enemy> Enemies { get; set; }
    public List<Item> Items { get; set; }
    public List<Door> Doors { get; set; }
    public List<Room> Rooms { get; set; }
    public List<Corridor> Corridors { get; set; }
    public Player Player { get; set; }
    public Statistics Stats { get; set; }

    public SessionDataJSON(SessionData sessionData) {
      FieldData = [..Enumerable.Range(0, sessionData.Field.GetLength(0))
                       .Select(i => Enumerable.Range(0, sessionData.Field.GetLength(1))
                                        .Select(j => sessionData.Field[i, j])
                                        .ToList())];
      Enemies = sessionData.Enemies;
      Items = sessionData.Items;
      Doors = sessionData.Doors;
      Rooms = sessionData.Rooms;
      Corridors = sessionData.Corridors;
      Player = sessionData.Player;
      Stats = sessionData.Stats;
    }

    public SessionDataJSON() {
      FieldData = [];
      Enemies = [];
      Items = [];
      Doors = [];
      Rooms = [];
      Corridors = [];
      Player = new(0, 0);
      Stats = new();
    }
  }
  public static SessionData LoadData(Level lvl, Player p, Statistics s) {
    SessionData sessionData = new();
    sessionData.Field = lvl.field;
    sessionData.Enemies = lvl.enemies;
    sessionData.Items = lvl.items;
    sessionData.Doors = lvl.doors;
    sessionData.Rooms = lvl.rooms;
    sessionData.Corridors = lvl.corridors;
    sessionData.Player = p;
    sessionData.Stats = s;
    SessionDataJSON sessionDataJSON = new SessionDataJSON(sessionData);
    SaveSessionData(sessionDataJSON);
    return sessionData;
  }

  public static void SaveSessionData(SessionDataJSON sessionDataJSON) {
    string savePath = FindCorrectFilePath();
    string json = JsonConvert.SerializeObject(
        sessionDataJSON, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

    Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
    File.WriteAllText(savePath, json);
  }

  public static SessionData? GetSessionData() {
    SessionDataJSON? sessionDataJSON = new();
    SessionData? sessionData = new();
    string savePath = FindCorrectFilePath();
    if (File.Exists(savePath)) {
      string json = File.ReadAllText(savePath);
      sessionDataJSON = JsonConvert.DeserializeObject<SessionDataJSON>(
          json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
      sessionData = ConvertSessionData(sessionDataJSON);
    } else {
      sessionData = null;
    }
    return sessionData;
  }

  public static SessionData ConvertSessionData(SessionDataJSON sessionDataJSON) {
    SessionData sessionData = new();

    sessionDataJSON.FieldData.SelectMany((row, i) => row.Select((val, j) => (i, j, val)))
        .ToList()
        .ForEach(t => sessionData.Field[t.i, t.j] = t.val);
    sessionData.Enemies = sessionDataJSON.Enemies;
    sessionData.Items = sessionDataJSON.Items;
    sessionData.Doors = sessionDataJSON.Doors;
    sessionData.Rooms = sessionDataJSON.Rooms;
    sessionData.Corridors = sessionDataJSON.Corridors;
    sessionData.Player = sessionDataJSON.Player;
    sessionData.Stats = sessionDataJSON.Stats;

    return sessionData;
  }

  public static string FindCorrectFilePath() {
    string projectRoot = Path.GetFullPath("../");
    string path = Path.Combine(projectRoot, "saves", "SessionData.json");
    return path;
  }
}