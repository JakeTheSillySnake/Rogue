namespace rogue.Domain;

using System.Collections.Generic;

enum Colors {
  BLUE = 1,
  WHITE,
  RED,
  YELLOW,
  GREEN,
}

public class Entity {
  public int x, y, hp, hp_max, str, agl, color;
  public string symbol = "";
  public static int valLow = 3, valMid = 5, valHigh = 10;

  public Entity() {}

  public void InitCoords(int x, int y) {
    this.x = x;
    this.y = y;
  }

  public int DistanceToTarget(Level lvl, int x, int y) {
    int distY = Math.Abs(this.y - y);
    int distX = Math.Abs(this.x - x);
    int dist = (int)Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));

    // TODO: dist is 1000 if no path exists
    return dist;
  }

  public bool CheckRight(Level lvl, int dist) {
    if (lvl.field[y, x + dist] == (int)CellStates.EMPTY || lvl.field[y, x + dist] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public bool CheckLeft(Level lvl, int dist) {
    if (lvl.field[y, x - dist] == (int)CellStates.EMPTY || lvl.field[y, x - dist] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public bool CheckUp(Level lvl, int dist) {
    if (lvl.field[y - dist, x] == (int)CellStates.EMPTY || lvl.field[y - dist, x] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public bool CheckDown(Level lvl, int dist) {
    if (lvl.field[y + dist, x] == (int)CellStates.EMPTY || lvl.field[y + dist, x] >= Level.itemCode)
      return true;
    else
      return false;
  }
}

public class Player : Entity {
  public int lvl = 1;
  public bool asleep = false;
  public Inventory backpack = new();
  public Weapon currWeapon = new();

  public Player(int x, int y) {
    symbol = "p";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valHigh;
    agl = valHigh;
    color = (int)Colors.BLUE;
    InitCoords(x, y);
  }

  public List<int> Move(int action, Level lvl) {
    List<int> res = [0, 0];
    if (asleep) {
      asleep = false;
      return res;
    }
    if (action == 'a' || action == 'A') {
      if (CheckLeft(lvl, 1))
        x--;
      else
        res = Attack(lvl, x - 1, y);
    }
    if (action == 'd' || action == 'D') {
      if (CheckRight(lvl, 1))
        x++;
      else
        res = Attack(lvl, x + 1, y);
    }
    if (action == 'w' || action == 'W') {
      if (CheckUp(lvl, 1))
        y--;
      else
        res = Attack(lvl, x, y - 1);
    }
    if (action == 's' || action == 'S') {
      if (CheckDown(lvl, 1))
        y++;
      else
        res = Attack(lvl, x, y + 1);
    }
    return res;
  }

  public (bool, int) ProcessDamage(int damage, string type) {
    // successful vampire attack
    if (damage > 0 && type == "v")
      hp_max -= 2;
    else if (damage > 0 && type == "s") {
      // successful snake attack
      Random rnd = new();
      if (rnd.Next(1, 4) == 1)
        asleep = true;
    }
    hp -= damage;
    if (hp < 0)
      hp = 0;
    if (hp == 0)
      return (true, damage);
    else
      return (false, damage);
  }

  public List<int> Attack(Level lvl, int targetX, int targetY) {
    List<int> res = [0, 0];
    if (lvl.field[targetY, targetX] == (int)CellStates.WALL)
      return res;
    int pos = lvl.field[targetY, targetX] - Level.enemyCode;
    int enemyAgl = lvl.enemies[pos].agl, chance;
    Random rnd = new();
    if (enemyAgl < agl)
      chance = 40;
    else if (enemyAgl == agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance)
      res[1] = str;  // hit
    else
      res[1] = 0;  // miss
    res[0] = lvl.field[targetY, targetX];
    return res;
  }

  public int CollectItem(Level lvl, int targetX, int targetY) {
    if (lvl.field[targetY, targetX] < Level.itemCode)
      return -1;
    int pos = lvl.field[targetY, targetX] - Level.itemCode;
    var i = lvl.items[pos];
    if (!i.active)
      return -1;
    if (i is Treasure)
      AddTreasure(i.value);
    else if (!backpack.AddItem(i))
      return -1;
    i.active = false;
    i.symbol = "";
    return pos;
  }

  public void AddTreasure(int num) {
    backpack.AddTreasure(num);
  }

  public int GetTreasure() {
    return backpack.GetTreasure();
  }
}