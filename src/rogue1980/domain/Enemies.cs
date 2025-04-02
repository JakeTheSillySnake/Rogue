namespace Domain.Enemies;

using Domain.Player;
using Domain.Level;

public abstract class Enemy : Entity {
  public int enmity;
  public bool asleep = false, follow = false, dead = false;

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
      return str;  // hit
    else
      return 0;  // miss
  }

  public int Act(Level lvl, Player p) {
    if (asleep || dead) {
      asleep = false;
      return 0;
    }
    int dist = DistanceToTarget(lvl, p.x, p.y), damage = 0;
    if (dist <= 1) {
      follow = true;
      damage = Attack(p);
    }
    if ((dist <= enmity || follow) && dist > 0 && (x != p.x || y != p.y)) {
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
    Random rnd = new Random();
    return rnd.Next(hp_max, str + agl + enmity + hp_max);
  }
}

public class Zombie : Enemy {
  private int _dirX = 1;

  public Zombie(int x, int y) {
    symbol = "z";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valMid;
    agl = valLow;
    color = (int)Colors.GREEN;
    enmity = 2;
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
  private int _dirX = 0, _dirY = 1;
  private bool _firstMove = true;
  // first player attack is miss

  public Vampire(int x, int y) {
    symbol = "v";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valMid;
    agl = valHigh;
    color = (int)Colors.RED;
    enmity = 3;
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
  private int _dir = 0;
  private bool _counterAttack = false;

  public Ogre(int x, int y) {
    symbol = "o";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valHigh;
    agl = valLow;
    color = (int)Colors.YELLOW;
    enmity = 2;
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

  public Ghost(int x, int y) {
    symbol = "g";
    hp = 4 * valLow;
    hp_max = 4 * valLow;
    str = valLow;
    agl = valHigh;
    color = (int)Colors.WHITE;
    enmity = 1;
    InitCoords(x, y);
    // improve later
    LoadRoom(4, 1, 65, 17);
  }

  public override void Move(Level lvl) {
    if (_timer == 0) {
      Random rnd = new Random();
      x = rnd.Next(_minX, _maxX + 1);
      y = rnd.Next(_minY, _maxY + 1);
      _timer = 6;
      // 20% chance to become invisible
      if (rnd.Next(1, 5) == 2 && !follow)
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

  public Snake(int x, int y) {
    symbol = "s";
    hp = 4 * valMid;
    hp_max = 4 * valMid;
    str = valMid;
    agl = valHigh;
    color = (int)Colors.WHITE;
    enmity = 3;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_steps > 0 && lvl.field[y + _dirY, x] == (int)CellStates.EMPTY &&
        lvl.field[y, x + _dirX] == (int)CellStates.EMPTY) {
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
  public Mimic(int x, int y) {
    symbol = "п";  // table or smth
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valLow;
    agl = valHigh;
    color = (int)Colors.WHITE;
    enmity = 1;
    InitCoords(x, y);
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
      return str;  // hit
    else
      return 0;  // miss
  }
}