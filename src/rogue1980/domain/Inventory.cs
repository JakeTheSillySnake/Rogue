namespace Domain.Inventory;

using System.Collections.Generic;

public class Inventory {
  public Dictionary<string, int> potions = new Dictionary<string, int>(9);
  public Dictionary<string, int> scrolls = new Dictionary<string, int>(9);
  public Dictionary<string, int> food = new Dictionary<string, int>(9);
  public Dictionary<string, int> weapons = new Dictionary<string, int>(9);
  public int treasure = 0;

  public Inventory() {}

  public void AddItem(int code) {}
  public void DeletItem(int code) {}
}