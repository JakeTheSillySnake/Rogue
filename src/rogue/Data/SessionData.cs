namespace rogue.Data;

using rogue.Domain;
using rogue.Domain.LevelMap;

public class SessionData {
  public int[,] field = new int[Level.ROWS, Level.COLS];
  public List<Enemy> enemies = [];
  public List<Item> items = [];
  public List<Door> doors = [];
  public List<Room> rooms = [];
  public List<(Route, int)> corridors = [];
  public Player player = new(0, 0);
  public Statistics stats = new();

  public SessionData() {}

  public void LoadData(Level lvl, Player p, Statistics s) {
    field = lvl.field;
    enemies = lvl.enemies;
    items = lvl.items;
    doors = lvl.doors;
    rooms = lvl.rooms;
    corridors = lvl.corridors;
    player = p;
    stats = s;
    SaveSessionData();
  }

  public void SaveSessionData() {
    // data to JSON saver
  }

  public bool GetSessionData() {
    bool ok = false;
    // get data from JSON file
    // if session not found/error --> ok = false
    return ok;
  }
}