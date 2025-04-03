namespace rogue.Domain;

using System.Collections.Generic;

enum Items { WEAPON = 0, POTION, SCROLL, FOOD, TREASURE }

enum Effects { Healing = 0, Strength, Agility }

public class Item {
  public int value, x = 0, y = 0;
  public bool active = true;
  public string symbol = "", name = "";

  public Item() {}

  public void Spawn(int x, int y) {
    this.x = x;
    this.y = y;
  }
}

public class Weapon : Item {
  private readonly string[] _weapons = ["Knife", "Spear", "Sword", "Axe", "Mace"];

  public Weapon() {
    Random rnd = new();
    int idx = rnd.Next(_weapons.Length);
    name = _weapons[idx];
    value = Entity.valMid + idx;
    symbol = "!";
  }
}

public class Potion : Item {
  public int type, effectLen = 10;

  public Potion() {
    Random rnd = new();
    type = rnd.Next(Enum.GetNames(typeof(Effects)).Length);
    name = Enum.GetNames(typeof(Effects))[type];
    value = Entity.valMid;
    symbol = "d";
  }
}

public class Scroll : Item {
  public int type;

  public Scroll() {
    Random rnd = new();
    type = rnd.Next(Enum.GetNames(typeof(Effects)).Length);
    name = Enum.GetNames(typeof(Effects))[type];
    value = Entity.valMid;
    symbol = "=";
  }
}

public class Food : Item {
  private readonly string[] _food = ["Apple", "Bread", "Steak"];

  public Food() {
    Random rnd = new();
    int idx = rnd.Next(_food.Length);
    name = _food[idx];
    value = Entity.valMid + (idx * 2);
    symbol = "+";
  }
}

public class Treasure : Item {
  public Treasure() {
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
  public void DeletItem(int pos) {}
}