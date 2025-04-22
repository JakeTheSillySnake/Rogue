namespace rogue.Domain;

using rogue.Domain.LevelMap;

enum Enemies { ZOMBIE = 0, VAMPIRE, OGRE, GHOST, SNAKE, MIMIC }

public abstract class Enemy : Entity {
  public int hostility { get; set; }
  public bool asleep { get; set; }
  public bool follow { get; set; }
  public bool dead { get; set; }

  public Enemy() {}

  public abstract void Move(Level lvl);

  public override bool CheckRight(Level lvl, int dist) {
    if (lvl.field[y, x + dist] < (int)MapCellStates.EXIT ||
        lvl.field[y, x + dist] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public override bool CheckLeft(Level lvl, int dist) {
    if (lvl.field[y, x - dist] < (int)MapCellStates.EXIT ||
        lvl.field[y, x - dist] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public override bool CheckUp(Level lvl, int dist) {
    if (lvl.field[y - dist, x] < (int)MapCellStates.EXIT ||
        lvl.field[y - dist, x] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public override bool CheckDown(Level lvl, int dist) {
    if (lvl.field[y + dist, x] < (int)MapCellStates.EXIT ||
        lvl.field[y + dist, x] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public virtual int Attack(Player p) {
    int chance = 0;
    Random rnd = new();
    if (agl < p.agl)
      chance = 40;
    else if (agl == p.agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance)
      return str;  // hit
    else
      return 0;  // miss
  }

  public int Act(Level lvl, Player p) {
    if (asleep || dead) {
      asleep = false;
      return 0;
    }
    int dist = DistanceToTarget(p.x, p.y), damage = 0;
    if (dist <= 1) {
      follow = true;
      damage = Attack(p);
    }
    if ((dist <= hostility || follow) && dist > 0 && (x != p.x || y != p.y)) {
      int initX = x, initY = y;
      // follow player
      follow = true;
      if (p.x > x && (p.y != y || p.x - 1 != x) && CheckRight(lvl, 1))
        x++;
      if (p.x < x && (p.y != y || p.x + 1 != x) && CheckLeft(lvl, 1))
        x--;
      if (p.y > y && (p.x != x || p.y - 1 != y) && CheckDown(lvl, 1))
        y++;
      if (p.y < y && (p.x != x || p.y + 1 != y) && CheckUp(lvl, 1))
        y--;
      if (x == initX && y == initY) {
        // ignore player if no path exists
        follow = false;
      }
    } else
      Move(lvl);
    return damage;
  }

  public virtual bool ProcessDamage(int damage) {
    hp -= damage;
    if (hp <= 0) {
      symbol = "";
      dead = true;
    }
    return dead;
  }

  public int GenTreasure() {
    Random rnd = new();
    return rnd.Next(hp_max, str + agl + hostility + hp_max);
  }
}

public class Zombie : Enemy {
  private int _dirX { get; set; }

  public Zombie(int x, int y) {
    follow = false;
    dead = false;
    asleep = false;
    _dirX = 1;
    symbol = "z";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valMid;
    agl = valLow;
    color = (int)Colors.GREEN;
    hostility = 2;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_dirX == 1 && CheckLeft(lvl, 1))
      x--;
    else if (_dirX == -1 && CheckRight(lvl, 1))
      x++;
    else
      _dirX *= -1;
  }
}

public class Vampire : Enemy {
  private int _dirX { get; set; }
  private int _dirY { get; set; }
  private bool _firstMove { get; set; }
  // first player attack is miss

  public Vampire(int x, int y) {
    follow = false;
    dead = false;
    asleep = false;
    _dirX = 0;
    _dirY = 1;
    _firstMove = true;
    symbol = "v";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valMid;
    agl = valHigh;
    color = (int)Colors.RED;
    hostility = 3;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_dirY == 1 && CheckDown(lvl, 1))
      y++;
    else if (CheckUp(lvl, 1))
      y--;
    _dirY *= -1;

    if (_dirX < 2 && CheckLeft(lvl, 1))
      x--;
    else if (CheckRight(lvl, 1))
      x++;
    _dirX++;
    if (_dirX > 3)
      _dirX = 0;
  }

  public override bool ProcessDamage(int damage) {
    if (_firstMove) {
      _firstMove = false;
      return dead;
    }
    hp -= damage;
    if (hp <= 0) {
      symbol = "";
      dead = true;
    }
    return dead;
  }
}

public class Ogre : Enemy {
  private int _dir { get; set; }
  private bool _counterAttack { get; set; }

  public Ogre(int x, int y) {
    follow = false;
    dead = false;
    asleep = false;
    _dir = 0;
    _counterAttack = false;
    symbol = "o";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valHigh;
    agl = valLow;
    color = (int)Colors.YELLOW;
    hostility = 2;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_dir == 0 && CheckRight(lvl, 1) && CheckRight(lvl, 2))
      x += 2;
    else if (_dir == 1 && CheckDown(lvl, 1) && CheckDown(lvl, 2))
      y += 2;
    else if (_dir == 2 && CheckLeft(lvl, 1) && CheckLeft(lvl, 2))
      x -= 2;
    else if (CheckUp(lvl, 1) && CheckUp(lvl, 2))
      y -= 2;
    _dir++;
    if (_dir > 3)
      _dir = 0;
  }

  public override int Attack(Player p) {
    int chance = 0;
    Random rnd = new();
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
  private int _timer { get; set; }
  private int _minX { get; set; }
  private int _maxX { get; set; }
  private int _minY { get; set; }
  private int _maxY { get; set; }

  public Ghost(int x, int y) {
    follow = false;
    dead = false;
    asleep = false;
    _timer = 5;
    symbol = "g";
    hp = 4 * valLow;
    hp_max = 4 * valLow;
    str = valLow;
    agl = valHigh;
    color = (int)Colors.WHITE;
    hostility = 1;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    LoadRoom(lvl);
    if (_timer == 0) {
      Random rnd = new();
      do {
        x = rnd.Next(_minX, _maxX + 1);
        y = rnd.Next(_minY, _maxY + 1);
      } while (lvl.field[y, x] == (int)MapCellStates.EXIT);
      _timer = 6;
      // 20% chance to become invisible
      if (rnd.Next(1, 5) == 2 && !follow)
        symbol = "";
      else
        symbol = "g";
    } else
      _timer--;
  }

  public void LoadRoom(Level lvl) {
    int minX = x, maxX = x, minY = y, maxY = y;
    while (CheckLeft(lvl, 1) || lvl.field[y, minX - 1] >= Level.enemyCode) minX--;
    while (CheckRight(lvl, 1) || lvl.field[y, minX + 1] >= Level.enemyCode) maxX++;
    while (CheckUp(lvl, 1) || lvl.field[minY - 1, x] >= Level.enemyCode) minY--;
    while (CheckDown(lvl, 1) || lvl.field[maxY + 1, x] >= Level.enemyCode) maxY++;
    _minX = minX;
    _maxX = maxX;
    _minY = minY;
    _maxY = maxY;
  }
}

public class Snake : Enemy {
  private int _dirX { get; set; }
  private int _dirY { get; set; }
  private int _steps { get; set; }

  public Snake(int x, int y) {
    follow = false;
    dead = false;
    asleep = false;
    _dirX = 1;
    _dirY = -1;
    _steps = 3;
    symbol = "s";
    hp = 4 * valMid;
    hp_max = 4 * valMid;
    str = valMid;
    agl = valHigh;
    color = (int)Colors.WHITE;
    hostility = 3;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_steps > 0 &&
        (lvl.field[y + _dirY, x] <= (int)MapCellStates.EXIT ||
         lvl.field[y + _dirY, x] >= Level.itemCode) &&
        (lvl.field[y, x + _dirX] <= (int)MapCellStates.EXIT ||
         lvl.field[y + _dirY, x] >= Level.itemCode)) {
      x += _dirX;
      y += _dirY;
      _steps--;
    } else {
      _steps = 3;
      Random rnd = new();
      _dirX = rnd.Next(2) == 1 ? 1 : -1;
      _dirY = rnd.Next(2) == 1 ? 1 : -1;
    }
  }
}

public class Mimic : Enemy {
  private readonly string[] _items = ["!", "=", "+", "d"];
  public Mimic(int x, int y) {
    follow = false;
    dead = false;
    asleep = false;
    Random rnd = new();
    symbol = _items[rnd.Next(_items.Length)];
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valLow;
    agl = valHigh;
    color = (int)Colors.WHITE;
    hostility = 1;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {}

  public override int Attack(Player p) {
    symbol = "m";
    int chance;
    Random rnd = new();
    if (agl < p.agl)
      chance = 40;
    else if (agl == p.agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance)
      return str;  // hit
    else
      return 0;  // miss
  }
}