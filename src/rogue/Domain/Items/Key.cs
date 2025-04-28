namespace rogue.Domain.Items;

using rogue.Domain.LevelMap;

public class Key : Item {
  public Key(int Color) {
    Type = "Key";
    if (Color == (int)Colors.RED)
      Subtype = "Red";
    else if (Color == (int)Colors.BLUE)
      Subtype = "Blue";
    else if (Color == (int)Colors.GREEN)
      Subtype = "Green";
    Symbol = "k";
    Value = Color;
  }
}