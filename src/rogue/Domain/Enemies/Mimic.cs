namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Mimic : Enemy {
  private readonly string[] _items = ["!", "=", "+", "d"];
  public Mimic(int x, int y) {
    Random rnd = new();
    Symbol = _items[rnd.Next(_items.Length)];
    Hp = 4 * valHigh;
    Hp_max = 4 * valHigh;
    Str= valLow;
    Agl = valHigh;
    Color = (int)Colors.WHITE;
    Hostility = 1;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {}

  public override int Attack(Player p) {
    Symbol = "m";
    int chance;
    Random rnd = new();
    if (Agl < p.Agl)
      chance = 40;
    else if (Agl == p.Agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance)
      return Str;  // hit
    else
      return 0;  // miss
  }
}