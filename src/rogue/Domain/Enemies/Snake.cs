namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Snake : Enemy {
  private int _dirX { get; set; } = 1;
  private int _dirY { get; set; } = -1;
  private int _steps { get; set; } = 3;

  public Snake(int x, int y) {
    Symbol = "s";
    Hp = 4 * valMid;
    Hp_max = 4 * valMid;
    Str = valMid;
    Agl = valHigh;
    Color = (int)Colors.WHITE;
    Hostility = 4;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_steps > 0 &&
        (lvl.field[PosY + _dirY, PosX] < (int)MapCellStates.EXIT ||
         lvl.field[PosY + _dirY, PosX] >= Level.itemCode) &&
        (lvl.field[PosY, PosX + _dirX] < (int)MapCellStates.EXIT ||
         lvl.field[PosY, PosX + _dirX] >= Level.itemCode) &&
        (lvl.field[PosY + _dirY, PosX + _dirX] < (int)MapCellStates.EXIT ||
         lvl.field[PosY + _dirY, PosX + _dirX] >= Level.itemCode)) {
      PosX += _dirX;
      PosY += _dirY;
      _steps--;
    } else {
      _steps = 3;
      Random rnd = new();
      _dirX = rnd.Next(2) == 1 ? 1 : -1;
      _dirY = rnd.Next(2) == 1 ? 1 : -1;
    }
  }
}