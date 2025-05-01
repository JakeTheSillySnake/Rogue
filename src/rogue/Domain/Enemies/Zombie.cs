namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Zombie : Enemy {
  private int _dirX { get; set; } = 1;

  public Zombie(int x, int y) {
    Symbol = "z";
    Hp = 4 * valHigh;
    Hp_max = 4 * valHigh;
    Str = valMid;
    Agl = valLow;
    Color = (int)Colors.GREEN;
    Hostility = 2;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_dirX == 1 && CheckLeft(lvl, 1))
      PosX--;
    else if (_dirX == -1 && CheckRight(lvl, 1))
      PosX++;
    else
      _dirX *= -1;
  }
}