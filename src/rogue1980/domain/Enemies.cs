namespace Domain.Enemies;

using Domain.Player;
using Domain.Level;

public abstract class Enemy : Entity {
  public int enmity;
  public bool asleep = false, follow = false;

  public Enemy() {}

  public abstract void Move(Level lvl);

  public virtual int Attack(Player p) {
    int chance = 0;
    Random rnd = new Random();
    if (agl < p.agl)
      chance = 40;
    else if (agl == p.agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance) 
      return str; // hit
    else
      return 0; // miss
  }

  public int Act(Level lvl, Player p) {
    if (asleep) {
      asleep = false;
      return 0;
    }
    int dist = DistanceToTarget(lvl, p.x, p.y);
    if (dist <= 1) {
      follow = true;
      return Attack(p);
    } 
    if ((dist <= enmity || follow) && dist > 0 && (x != p.x || y != p.y)) {
      // follow
      follow = true;
      if (p.x > x && (p.y != y || p.x - 1 != x))
        x++;
      if (p.x < x && (p.y != y || p.x + 1 != x))
        x--;
      if (p.y > y && (p.x != x || p.y - 1 != y))
        y++;
      if (p.y < y && (p.x != x || p.y + 1 != y))
        y--;
    } else
      Move(lvl);
    return 0;
  }
}

public class Zombie : Enemy {
  private int _dirX = 1;

  public Zombie() {
    symbol = "z";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valMid;
    agl = valLow;
    color = (int)Colors.GREEN;
    enmity = 2;
    InitCoords(12, 5);
  }

  public override void Move(Level lvl) {
    if (_dirX == 1 && lvl.field[y, x - 1] == Level.EMPTY)
      x--;
    else if (_dirX == -1 && lvl.field[y, x + 1] == Level.EMPTY)
      x++;
    else
      _dirX *= -1;
  }
}

public class Vampire : Enemy {
  private int _dirX = 0, _dirY = 1;
  private bool _firstMove = true; 
  // first player attack is miss

  public Vampire() {
    symbol = "v";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valMid;
    agl = valHigh;
    color = (int)Colors.RED;
    enmity = 3;
    InitCoords(12, 5);
  }

  public override void Move(Level lvl) {
    if (_dirY == 1 && lvl.field[y + 1, x] == Level.EMPTY)
      y++;
    else if (lvl.field[y - 1, x] == Level.EMPTY)
      y--;
    _dirY *= -1;

    if (_dirX < 2 && lvl.field[y, x - 1] == Level.EMPTY)
      x--;
    else if (lvl.field[y, x + 1] == Level.EMPTY)
      x++;
    _dirX++;
    if (_dirX > 3)
      _dirX = 0;
  }
}

public class Ogre : Enemy {
  private int _dir = 0;
  private bool _counterAttack = false;

  public Ogre() {
    symbol = "o";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valHigh;
    agl = valLow;
    color = (int)Colors.YELLOW;
    enmity = 2;
    InitCoords(12, 5);
  }

  public override void Move(Level lvl) {
    if (_dir == 0 && lvl.field[y, x + 1] == Level.EMPTY
    && lvl.field[y, x + 2] == Level.EMPTY)
      x += 2;
    else if (_dir == 1 && lvl.field[y + 1, x] == Level.EMPTY
    && lvl.field[y + 2, x] == Level.EMPTY)
      y += 2;
    else if (_dir == 2 && lvl.field[y, x - 1] == Level.EMPTY
    && lvl.field[y, x - 2] == Level.EMPTY)
      x -= 2;
    else if (lvl.field[y - 1, x] == Level.EMPTY
    && lvl.field[y - 2, x] == Level.EMPTY)
      y -= 2;
    _dir++;
    if (_dir > 3)
      _dir = 0;
  }

  public override int Attack(Player p) {
    int chance = 0;
    Random rnd = new Random();
    if (agl < p.agl)
      chance = 40;
    else if (agl == p.agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    int damage = 0;
    if (hitOrMiss <= chance || _counterAttack)
      damage = str;
    // sleep after every attack
    asleep = true;
    // every attack after first lands
    if (!_counterAttack)
      _counterAttack = true;
    return damage;
  }
}

public class Ghost : Enemy {
  private int _timer = 5, _minX, _maxX, _minY, _maxY;

  public Ghost() {
    symbol = "g";
    hp = valLow;
    hp_max = valLow;
    str = valLow;
    agl = valHigh;
    color = (int)Colors.WHITE;
    enmity = 1;
    InitCoords(12, 14);
  }

  public override void Move(Level lvl) {
    // needs loaded room
    if (_timer == 0) {
      Random rnd = new Random();
      x = rnd.Next(_minX, _maxX + 1);
      y = rnd.Next(_minY, _maxY + 1);
      _timer = 5;
      // sometimes becomes invisible
      if (rnd.Next(1, 5) == 2)
        symbol = "";
      else
        symbol = "g";
    } else
      _timer--;
  }

  public void LoadRoom(int startX, int startY, int endX, int endY) {
    _minX = startX + 1;
    _maxX = endX - 1;
    _minY = startY + 1;
    _maxY = endY - 1;
  }
}

public class Snake : Enemy {
  private int _dirX = 1, _dirY = -1, _steps = 3;

  public Snake() {
    symbol = "s";
    hp = valMid;
    hp_max = valMid;
    str = valMid;
    agl = valHigh;
    color = (int)Colors.WHITE;
    enmity = 3;
    InitCoords(12, 14);
  }

  public override void Move(Level lvl) {
    if (_steps > 0 && lvl.field[y + _dirY, x] == Level.EMPTY &&
        lvl.field[y, x + _dirX] == Level.EMPTY) {
      x += _dirX;
      y += _dirY;
      _steps--;
    } else {
      _steps = 3;
      Random rnd = new Random();
      _dirX = rnd.Next(2) == 1 ? 1 : -1;
      _dirY = rnd.Next(2) == 1 ? 1 : -1;
    }
  }
}

public class Mimic : Enemy {
  public Mimic() {
    symbol = "п"; // table idk
    hp = valHigh;
    hp_max = valHigh;
    str = valLow;
    agl = valHigh;
    color = (int)Colors.WHITE;
    enmity = 1;
    InitCoords(57, 14);
  }

  public override void Move(Level lvl) {}

  public override int Attack(Player p) {
    symbol = "m";
    int chance = 0;
    Random rnd = new Random();
    if (agl < p.agl)
      chance = 40;
    else if (agl == p.agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance) 
      return str; // hit
    else
      return 0; // miss
  }
}