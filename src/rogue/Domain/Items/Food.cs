namespace rogue.Domain.Items;

public class Food : Item {
  private readonly string[] _food = ["Apple", "Bread", "Chicken", "Cake", "Steak"];

  public Food() {
    Random rnd = new();
    int idx = rnd.Next(_food.Length);
    Type = "Food";
    Subtype = "Health";
    Name = _food[idx];
    Value = Entity.valLow + idx;
    Symbol = "+";
  }
}