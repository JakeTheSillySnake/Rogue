namespace rogue.Data;

using rogue.Domain;
using rogue.Domain.LevelMap;

public class SessionData {
    public int[,] Field { get; set; } = new int[Level.ROWS, Level.COLS];
    public List<Enemy> Enemies { get; set; } = new();
    public List<Item> Items { get; set; } = new();
    public List<Door> Doors { get; set; } = new();
    public List<Room> Rooms { get; set; } = new();
    public List<Corridor> Corridors { get; set; } = new();
    public Player Player { get; set; } = new(0, 0);
    public Statistics Stats { get; set; } = new();
}