namespace rogue.Domain;

using rogue.Domain.LevelMap;

public class Entity {
  public int PosX { get; set; } = 0;
  public int PosY { get; set; } = 0;
  public int Hp { get; set; } = 0;
  public int Hp_max { get; set; } = 0;
  public int Str { get; set; } = 0;
  public int Agl { get; set; } = 0;
  public int Color { get; set; } = 0;
  public string Symbol { get; set; } = "";
  public bool Asleep { get; set; } = false;
  public static int valLow = 3, valMid = 5, valHigh = 10;

  public Entity() {}

  public void InitCoords(int x, int y) {
    PosX = x;
    PosY = y;
  }

  public int DistanceToTarget(int x, int y) {
    int distY = Math.Abs(PosY - y);
    int distX = Math.Abs(PosX - x);
    int dist = (int)Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
    return dist;
  }

  public virtual bool CheckRight(Level lvl, int dist) {
    if (lvl.field[PosY, PosX + dist] < (int)MapCellStates.WALL ||
        lvl.field[PosY, PosX + dist] >= Level.itemCode ||
        lvl.field[PosY, PosX + dist] == (int)MapCellStates.DOOR)
      return true;
    else
      return false;
  }

  public virtual bool CheckLeft(Level lvl, int dist) {
    if (lvl.field[PosY, PosX - dist] < (int)MapCellStates.WALL ||
        lvl.field[PosY, PosX - dist] >= Level.itemCode ||
        lvl.field[PosY, PosX - dist] == (int)MapCellStates.DOOR)
      return true;
    else
      return false;
  }

  public virtual bool CheckUp(Level lvl, int dist) {
    if (lvl.field[PosY - dist, PosX] < (int)MapCellStates.WALL ||
        lvl.field[PosY - dist, PosX] >= Level.itemCode ||
        lvl.field[PosY - dist, PosX] == (int)MapCellStates.DOOR)
      return true;
    else
      return false;
  }

  public virtual bool CheckDown(Level lvl, int dist) {
    if (lvl.field[PosY + dist, PosX] < (int)MapCellStates.WALL ||
        lvl.field[PosY + dist, PosX] >= Level.itemCode ||
        lvl.field[PosY + dist, PosX] == (int)MapCellStates.DOOR)
      return true;
    else
      return false;
  }
}