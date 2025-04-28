namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Ogre : Enemy {
  private int _dir { get; set; } = 0;
  private bool _counterAttack { get; set; } = false;

  public Ogre(int x, int y) {
    Symbol = "o";
    Hp = 4 * valHigh;
    Hp_max = 4 * valHigh;
    Str= valHigh;
    Agl = valLow;
    Color = (int)Colors.YELLOW;
    Hostility = 2;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_dir == 0 && CheckRight(lvl, 1) && CheckRight(lvl, 2))
      PosX += 2;
    else if (_dir == 1 && CheckDown(lvl, 1) && CheckDown(lvl, 2))
      PosY += 2;
    else if (_dir == 2 && CheckLeft(lvl, 1) && CheckLeft(lvl, 2))
      PosX -= 2;
    else if (CheckUp(lvl, 1) && CheckUp(lvl, 2))
      PosY -= 2;
    _dir++;
    if (_dir > 3)
      _dir = 0;
  }

  public override int Attack(Player p) {
    int chance = 0;
    Random rnd = new();
    if (Agl < p.Agl)
      chance = 40;
    else if (Agl == p.Agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    int damage = 0;
    if (hitOrMiss <= chance || _counterAttack)
      damage = Str;
    // sleep after every attack
    Asleep = true;
    // every attack after first lands
    if (!_counterAttack)
      _counterAttack = true;
    return damage;
  }
}