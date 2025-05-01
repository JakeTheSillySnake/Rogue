namespace rogue.Domain.Items;

public class Potion : Item {
  public int EffectLen { get; set; } = 15;

  private readonly int[] _vals = [Entity.valLow, Entity.valMid, Entity.valHigh];
  private readonly string[] _power = ["Weak Potion", "Medium Potion", "Powerful Potion"];

  public Potion() {
    Random rnd = new();
    int idx = rnd.Next(Enum.GetNames(typeof(Effects)).Length);
    Subtype = Enum.GetNames(typeof(Effects))[idx];

    idx = rnd.Next(_vals.Length);
    Value = _vals[idx];
    Type = _power[idx];
    Symbol = "d";
  }
}