namespace rogue.Data;

using rogue.Domain;
using rogue.Domain.LevelMap;
using rogue.Domain.Enemies;
using rogue.Domain.Items;

public class SessionData {
  public int[,] Field { get; set; } = new int[Level.ROWS, Level.COLS];
  public List<Enemy> Enemies { get; set; } = [];
  public List<Item> Items { get; set; } = [];
  public List<Door> Doors { get; set; } = [];
  public List<Room> Rooms { get; set; } = [];
  public List<Corridor> Corridors { get; set; } = [];
  public Player Player { get; set; } = new(0, 0);
  public Statistics Stats { get; set; } = new();
}