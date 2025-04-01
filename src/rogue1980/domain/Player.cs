namespace Domain.Player;

using System.Collections.Generic;
using Domain.Level;

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
  public int valLow = 3, valMid = 5, valHigh = 10;

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
}

public class Player : Entity {
  public int lvl = 1;
  private bool _asleep = false;

  public Player() {
    symbol = "p";
    hp = 4 * valHigh;
    hp_max = 4 * valHigh;
    str = valHigh;
    agl = valHigh;
    color = (int)Colors.BLUE;
    InitCoords(34, 14);
  }
  public void Move(int action, Level lvl) {
    if (_asleep) {
        _asleep = false;
        return;
    }
    if ((action == 'a' || action == 'A') && lvl.field[y, x - 1] == Level.EMPTY)
      x--;
    if ((action == 'd' || action == 'D') && lvl.field[y, x + 1] == Level.EMPTY)
      x++;
    if ((action == 'w' || action == 'W') && lvl.field[y - 1, x] == Level.EMPTY)
      y--;
    if ((action == 's' || action == 'S') && lvl.field[y + 1, x] == Level.EMPTY)
      y++;
  }
  public bool TakeDamage(int damage, string type) {
    // successful vampire attack
    if (damage > 0 && type == "v")
      hp_max -= 2;
    else if (damage > 0 && type == "s") {
      // successful snake attack
      Random rnd = new Random();
      if (rnd.Next(1, 4) == 1)
        _asleep = true;
    }
    hp -= damage;
    if (hp < 0) hp = 0;
    if (hp == 0) return true;
    else return false;
  }
}

/*public class Backpack {
    public var potions = new Dictionary<String, Int>(9);
    public var scrolls = new Dictionary<String, Int>(9);
    public var food = new Dictionary<String, Int>(9);
    public int treasure = 0;
    public Backpack() {}

    public void ListInventory();
    public void AddItem();
    public void DeletItem();
}*/