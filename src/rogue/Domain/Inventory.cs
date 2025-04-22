namespace rogue.Domain;

using System.Collections.Generic;
using rogue.Domain.LevelMap;
using rogue.Data;

enum Items { WEAPON = 0, POTION, SCROLL, FOOD, TREASURE, KEY }

enum Effects { Health = 0, Strength, Agility }

public class Item {
  public int value { get; set; }
  public int x { get; set; }
  public int y { get; set; }
  public bool active { get; set; }
  public string symbol { get; set; }
  public string type { get; set; }
  public string subtype { get; set; }
  public string name { get; set; }

  public Item() {
    active = true;
    symbol = "";
    type = "";
    subtype = "";
    name = "";
  }

  public void Spawn(int x, int y) {
    this.x = x;
    this.y = y;
  }
}

public class Weapon : Item {
  private readonly string[] _weapons = ["Knife", "Spear", "Sword", "Axe", "Mace"];
  public bool equipped { get; set; }
  public int floorState { get; set; }

  public Weapon() {
    active = true;
    equipped = false;
    floorState = 0;
    Random rnd = new();
    int idx = rnd.Next(_weapons.Length);
    type = "Weapon";
    subtype = "Strength";
    name = _weapons[idx];
    value = Entity.valLow + idx;
    symbol = "!";
  }
}

public class Potion : Item {
  public int effectLen { get; set; }

  private readonly int[] _vals = [Entity.valLow, Entity.valMid, Entity.valHigh];
  private readonly string[] _power = ["Weak Potion", "Medium Potion", "Strong Potion"];

  public Potion() {
    active = true;
    effectLen = 15;
    Random rnd = new();
    int idx = rnd.Next(Enum.GetNames(typeof(Effects)).Length);
    subtype = Enum.GetNames(typeof(Effects))[idx];

    idx = rnd.Next(_vals.Length - 1);
    value = _vals[idx];
    type = _power[idx];
    symbol = "d";
  }
}

public class Scroll : Item {
  private readonly int[] _vals = [Entity.valLow, Entity.valMid, Entity.valHigh];
  private readonly string[] _power = ["Weak Scroll", "Medium Scroll", "Strong Scroll"];

  public Scroll() {
    active = true;
    Random rnd = new();
    int idx = rnd.Next(Enum.GetNames(typeof(Effects)).Length);
    subtype = Enum.GetNames(typeof(Effects))[idx];

    idx = rnd.Next(_vals.Length - 1);
    value = _vals[idx];
    type = _power[idx];
    symbol = "=";
  }
}

public class Food : Item {
  private readonly string[] _food = ["Apple", "Bread", "Chicken", "Cake", "Steak"];

  public Food() {
    active = true;
    Random rnd = new();
    int idx = rnd.Next(_food.Length);
    type = "Food";
    subtype = "Health";
    name = _food[idx];
    value = Entity.valLow + idx;
    symbol = "+";
  }
}

public class Key : Item {
  public Key(int color) {
    active = true;
    type = "Key";
    if (color == (int)Colors.RED)
      subtype = "Red";
    else if (color == (int)Colors.BLUE)
      subtype = "Blue";
    else if (color == (int)Colors.GREEN)
      subtype = "Green";
    symbol = "k";
    value = color;
  }
}

public class Treasure : Item {
  public Treasure() {
    active = true;
    type = "Treasure";
    value = 0;
    symbol = "@";
  }
}

public class Inventory {
  public Treasure treasure = new();
  public List<Weapon> weapons = [];
  public List<Potion> potions = [];
  public List<Scroll> scrolls = [];
  public List<Food> food = [];
  public List<Key> keys = [];

  public Inventory() {}

  public int GetTreasure() {
    return treasure.value;
  }

  public void AddTreasure(int num) {
    treasure.value += num;
  }

  public bool AddItem(Item i) {
    bool success = true;
    if (i is Weapon w) {
      if (weapons.Count == 9)
        success = false;
      else
        weapons.Add(w);
    } else if (i is Potion p) {
      if (potions.Count == 9)
        success = false;
      else
        potions.Add(p);
    } else if (i is Scroll s) {
      if (scrolls.Count == 9)
        success = false;
      else
        scrolls.Add(s);
    } else if (i is Food f) {
      if (food.Count == 9)
        success = false;
      else
        food.Add(f);
    } else if (i is Key k) {
      keys.Add(k);
    }
    return success;
  }

  public void RemoveItem(Item i, Statistics stats) {
    if (i is Weapon w) {
      weapons.Remove(w);
    } else if (i is Potion p) {
      potions.Remove(p);
      stats.Potions++;
    } else if (i is Scroll s) {
      scrolls.Remove(s);
      stats.Scrolls++;
    } else if (i is Food f) {
      food.Remove(f);
      stats.Food++;
    } else if (i is Key k) {
      keys.Remove(k);
    }
  }
}