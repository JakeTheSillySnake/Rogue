namespace rogue.Domain;

using System.Collections.Generic;

enum Items { WEAPON = 0, POTION, SCROLL, FOOD, TREASURE }

enum Effects { Health = 0, Strength, Agility }

public class Item {
  public int value, x = 0, y = 0;
  public bool active = true;
  public string symbol = "", type = "", subtype = "", name = "";

  public Item() {}

  public void Spawn(int x, int y) {
    this.x = x;
    this.y = y;
  }
}

public class Weapon : Item {
  private readonly string[] _weapons = ["Knife", "Spear", "Sword", "Axe", "Mace"];
  public bool equipped = false;

  public Weapon() {
    Random rnd = new();
    int idx = rnd.Next(_weapons.Length);
    type = "Weapon";
    subtype = "Strength";
    name = _weapons[idx];
    value = Entity.valMid + idx;
    symbol = "!";
  }
}

public class Potion : Item {
  public int effectLen = 15;

  private readonly int[] _vals = [Entity.valLow, Entity.valMid, Entity.valHigh];
  private readonly string[] _power = ["Weak Potion", "Medium Potion", "Strong Potion"];

  public Potion() {
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
    Random rnd = new();
    int idx = rnd.Next(_food.Length);
    type = "Food";
    subtype = "Health";
    name = _food[idx];
    value = Entity.valMid + idx;
    symbol = "+";
  }
}

public class Treasure : Item {
  public Treasure() {
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
    }
    return success;
  }

  public void RemoveItem(Item i) {
    if (i is Weapon w) {
      weapons.Remove(w);
    } else if (i is Potion p) {
      potions.Remove(p);
    } else if (i is Scroll s) {
      scrolls.Remove(s);
    } else if (i is Food f) {
      food.Remove(f);
    }
  }
}