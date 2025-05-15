namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Vampire : Enemy {
  private int _dirX { get; set; } = 0;
  private int _dirY { get; set; } = 1;
  public bool firstMove { get; set; } = true;
  // first player attack is miss

  public Vampire(int x, int y) {
    Symbol = "v";
    Hp = 4 * valHigh;
    Hp_max = 4 * valHigh;
    Str = valMid;
    Agl = valHigh;
    Color = (int)Colors.RED;
    Hostility = 4;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_dirY == 1 && CheckDown(lvl, 1))
      PosY++;
    else if (CheckUp(lvl, 1))
      PosY--;
    _dirY *= -1;

    if (_dirX < 2 && CheckLeft(lvl, 1))
      PosX--;
    else if (CheckRight(lvl, 1))
      PosX++;
    _dirX++;
    if (_dirX > 3)
      _dirX = 0;
  }

  public override bool ProcessDamage(int damage) {
    if (firstMove) {
      firstMove = false;
      return Dead;
    }
    Hp -= damage;
    if (Hp <= 0) {
      Symbol = "";
      Dead = true;
    }
    return Dead;
  }
}