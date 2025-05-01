namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Ghost : Enemy {
  private int _timer { get; set; } = 5;
  private int _minX { get; set; } = 0;
  private int _maxX { get; set; } = 0;
  private int _minY { get; set; } = 0;
  private int _maxY { get; set; } = 0;

  public Ghost(int x, int y) {
    Symbol = "g";
    Hp = 4 * valLow;
    Hp_max = 4 * valLow;
    Str = valLow;
    Agl = valHigh;
    Color = (int)Colors.WHITE;
    Hostility = 1;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    LoadRoom(lvl);
    if (_timer == 0) {
      Random rnd = new();
      do {
        PosX = rnd.Next(_minX, _maxX + 1);
        PosY = rnd.Next(_minY, _maxY + 1);
      } while (lvl.field[PosY, PosX] == (int)MapCellStates.EXIT);
      _timer = 6;
      // 20% chance to become invisible
      if (rnd.Next(1, 5) == 2 && !Follow)
        Symbol = "";
      else
        Symbol = "g";
    } else
      _timer--;
  }

  public void LoadRoom(Level lvl) {
    int minX = PosX, maxX = PosX, minY = PosY, maxY = PosY;
    while (CheckLeft(lvl, 1) || lvl.field[PosY, minX - 1] >= Level.enemyCode) minX--;
    while (CheckRight(lvl, 1) || lvl.field[PosY, minX + 1] >= Level.enemyCode) maxX++;
    while (CheckUp(lvl, 1) || lvl.field[minY - 1, PosX] >= Level.enemyCode) minY--;
    while (CheckDown(lvl, 1) || lvl.field[maxY + 1, PosX] >= Level.enemyCode) maxY++;
    _minX = minX;
    _maxX = maxX;
    _minY = minY;
    _maxY = maxY;
  }
}