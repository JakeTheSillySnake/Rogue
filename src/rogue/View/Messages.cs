namespace rogue.View;

using rogue.Data;
using rogue.Domain;
using rogue.Domain.LevelMap;
using rogue.Domain.Enemies;
using rogue.Domain.Items;

public class Messages {
  public Queue<string> messages = new();

  public Messages() {}

  public void Clear() {
    messages.Clear();
  }

  public void ProcessItemMessages(Level lvl, Player player, Statistics stats) {
    int pos = player.CollectItem(lvl, player.PosX, player.PosY);
    if (pos < 0)
      return;
    var i = lvl.items[pos];
    if (i is Weapon w)
      lvl.PickUpWeapon(w);
    string item = "";
    if (i is Treasure) {
      item = string.Format("{0} Coins", i.Value);
      stats.Treasure += i.Value;
    } else if (i is Potion || i is Scroll)
      item = string.Format("{0} of {1}", i.Type, i.Subtype);
    else if (i is Food || i is Weapon)
      item = string.Format("{0}", i.Name);
    else if (i is Key)
      item = string.Format("{0} Key", i.Subtype);
    messages.Enqueue(string.Format("You collected {0}!", item));
  }

  public (string, bool) ProcessDamageMessages(Level lvl, Player player, Statistics stats) {
    foreach (var e in lvl.enemies) {
      (bool, int)attack = (false, 0);
      string attacker = "";
      if (e is Zombie z) {
        attack = player.ProcessDamage(z.Act(lvl, player), z.Symbol);
        attacker = "Zombie";
      } else if (e is Vampire v) {
        attack = player.ProcessDamage(v.Act(lvl, player), v.Symbol);
        attacker = "Vampire";
      } else if (e is Ogre o) {
        attack = player.ProcessDamage(o.Act(lvl, player), o.Symbol);
        attacker = "Ogre";
      } else if (e is Ghost g) {
        attack = player.ProcessDamage(g.Act(lvl, player), g.Symbol);
        attacker = "Ghost";
      } else if (e is Snake s) {
        attack = player.ProcessDamage(s.Act(lvl, player), s.Symbol);
        attacker = "Snake-Wizard";
      } else if (e is Mimic m) {
        attack = player.ProcessDamage(m.Act(lvl, player), m.Symbol);
        attacker = "Mimic";
      }
      bool isOver = attack.Item1;
      lvl.UpdateField();
      if (attack.Item2 > 0) {
        messages.Enqueue(string.Format("You were hit by {0}! (-{1} Hp)", attacker, attack.Item2));
        stats.HitsReceived++;
      }
      if (isOver)
        return (attacker, isOver);
    }
    return ("", false);
  }

  public void ProcessAttackMessages(Level lvl, Player player, Statistics stats,
                                    List<int> attackResult, bool killEnemy) {
    if (attackResult[0] < Level.enemyCode)
      return;
    var e = lvl.enemies[attackResult[0] - Level.enemyCode];
    string enemy = "";
    if (e is Zombie) {
      enemy = "Zombie";
    } else if (e is Vampire) {
      enemy = "Vampire";
    } else if (e is Ogre) {
      enemy = "Ogre";
    } else if (e is Ghost) {
      enemy = "Ghost";
    } else if (e is Snake) {
      enemy = "Snake";
    } else if (e is Mimic) {
      enemy = "Mimic";
    }
    if (enemy != "" && attackResult[1] != 0) {
      messages.Enqueue(string.Format("You dealt {0} damage to {1}!", attackResult[1], enemy));
      stats.HitsDealt++;
      if (killEnemy) {
        messages.Enqueue(string.Format("You defeated {0}!", enemy));
        stats.Kills++;
      }
    } else if (enemy != "")
      messages.Enqueue(string.Format("You tried to hit {0} but missed!", enemy));
    if (player.Asleep)
      messages.Enqueue("You were stunned by Snake-Wizard!");
  }

  public void DrawStats(Statistics stats) {
    messages.Clear();
    messages.Enqueue(
        string.Format("Level: {0}, treasure collected: {1}", stats.Lvl, stats.Treasure));
    messages.Enqueue(string.Format("Used potions: {0}, scrolls: {1}, food: {2}", stats.Potions,
                                   stats.Scrolls, stats.Food));
    messages.Enqueue(string.Format("Mobs killed: {0}, hits dealt: {1}, hits received: {2}",
                                   stats.Kills, stats.HitsDealt, stats.HitsReceived));
    messages.Enqueue(string.Format("Distance traveled: {0}", stats.DistWalked));
  }

  public void ListInventory(Player player) {
    messages.Clear();
    messages.Enqueue(string.Format("Treasure: {0} Coins", player.GetTreasure()));
    messages.Enqueue(string.Format("Potions: {0} (Press K to Use)", player.backpack.potions.Count));
    messages.Enqueue(string.Format("Scrolls: {0} (Press E to Use)", player.backpack.scrolls.Count));
    messages.Enqueue(string.Format("Food: {0} (Press J to Use)", player.backpack.food.Count));
    messages.Enqueue(string.Format("Weapons: {0} (Press H to Use)", player.backpack.weapons.Count));
    messages.Enqueue(string.Format("Keys: {0} (Press L to Use)", player.backpack.keys.Count));
  }

  public void ListItems(Player player, List<Item> items, int type, string stype) {
    messages.Clear();
    if (items.Count == 0 && type != (int)Items.WEAPON) {
      messages.Enqueue("There is nothing here.");
      return;
    }
    if (items.Count == 0 && type == (int)Items.WEAPON && !player.currWeapon.Equipped) {
      messages.Enqueue("There is nothing here.");
      return;
    }
    int begin =
        (items.Count < 9 && type == (int)Items.WEAPON && player.currWeapon.Equipped) ? 0 : 1;
    messages.Enqueue(string.Format("Choose {0} | ESC to return:", stype));
    if (items.Count < 9 && type == (int)Items.WEAPON && player.currWeapon.Equipped) {
      messages.Enqueue("0. Unequip current weapon");
      begin++;
    }
    foreach (var i in items) {
      string name;
      if (i is Weapon || i is Food)
        name = i.Name;
      else if (i is Key)
        name = string.Format("{0} Key", i.Subtype);
      else
        name = string.Format("{0} of {1}", i.Type, i.Subtype);
      if (i is Key)
        messages.Enqueue(string.Format("{0}. {1}", begin, name));
      else
        messages.Enqueue(string.Format("{0}. {1} ({2} +{3})", begin, name, i.Subtype, i.Value));
      begin++;
    }
  }
}