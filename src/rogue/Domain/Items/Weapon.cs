namespace rogue.Domain.Items;

public class Weapon : Item {
  private readonly string[] _weapons = ["Knife", "Spear", "Sword", "Axe", "Mace"];
  public bool Equipped { get; set; } = false;
  public int FloorState { get; set; } = 0;

  public Weapon() {
    Random rnd = new();
    int idx = rnd.Next(_weapons.Length);
    Type = "Weapon";
    Subtype = "Strength";
    Name = _weapons[idx];
    Value = Entity.valLow + idx;
    Symbol = "!";
  }
}