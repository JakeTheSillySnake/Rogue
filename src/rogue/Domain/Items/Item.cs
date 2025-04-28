namespace rogue.Domain.Items;

enum Items { WEAPON = 0, POTION, SCROLL, FOOD, TREASURE, KEY }

enum Effects { Health = 0, Strength, Agility }

public class Item {
  public int Value { get; set; } = 0;
  public int PosX { get; set; } = 0;
  public int PosY { get; set; } = 0;
  public bool Active { get; set; } = true;
  public string Symbol { get; set; } = "";
  public string Type { get; set; } = "";
  public string Subtype { get; set; } = "";
  public string Name { get; set; } = "";

  public Item() {}

  public void Spawn(int x, int y) {
    PosX = x;
    PosY = y;
  }
}