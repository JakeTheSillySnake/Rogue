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
  public int valLow = 5, valMid = 7, valHigh = 10;

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
  // TODO: public abstract void Attack();
}

public class Player : Entity {
  public int lvl = 1;

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
    if ((action == 'a' || action == 'A') && lvl.field[y, x - 1] == Level.EMPTY)
      x--;
    if ((action == 'd' || action == 'D') && lvl.field[y, x + 1] == Level.EMPTY)
      x++;
    if ((action == 'w' || action == 'W') && lvl.field[y - 1, x] == Level.EMPTY)
      y--;
    if ((action == 's' || action == 'S') && lvl.field[y + 1, x] == Level.EMPTY)
      y++;
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