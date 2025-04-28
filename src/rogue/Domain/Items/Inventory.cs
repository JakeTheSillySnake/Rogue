namespace rogue.Domain.Items;

using System.Collections.Generic;
using rogue.Data;

public class Inventory {
  public Treasure treasure = new();
  public List<Weapon> weapons = [];
  public List<Potion> potions = [];
  public List<Scroll> scrolls = [];
  public List<Food> food = [];
  public List<Key> keys = [];

  public Inventory() {}

  public int GetTreasure() {
    return treasure.Value;
  }

  public void AddTreasure(int num) {
    treasure.Value += num;
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