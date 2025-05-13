namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Mimic : Enemy {
  private readonly string[] _items = ["!", "=", "+", "d"];
  private int _dirY { get; set; } = 1;

  public Mimic(int x, int y) {
    Random rnd = new();
    Symbol = _items[rnd.Next(_items.Length)];
    Hp = 4 * valHigh;
    Hp_max = 4 * valHigh;
    Str = valLow;
    Agl = valHigh;
    Color = (int)Colors.WHITE;
    Hostility = 2;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (Symbol != "m")
      return;
    if (_dirY == 1 && CheckUp(lvl, 1))
      PosY--;
    else if (_dirY == -1 && CheckDown(lvl, 1))
      PosY++;
    else
      _dirY *= -1;
  }

  public override void ChangeSymbol() {
    Symbol = "m";
  }
}