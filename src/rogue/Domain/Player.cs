namespace rogue.Domain;

using System.Collections.Generic;
using rogue.Domain.LevelMap;
using rogue.Domain.Items;
using rogue.Data;

public class Player : Entity {
  public int Lvl { get; set; } = 1;
  public string Effect { get; set; } = "";
  public int EffCount { get; set; } = 0;
  public Inventory backpack = new();
  public Weapon currWeapon = new();
  public Potion currPotion = new();

  public Player(int x, int y) {
    Symbol = "p";
    Hp = 2 * valHigh;
    Hp_max = 2 * valHigh;
    Str = valMid;
    Agl = valMid;
    Color = (int)Colors.BLUE;
    InitCoords(x, y);
  }

  public List<int> Move(int action, Level lvl, Statistics stats) {
    ProcessEffects();
    List<int> res = [0, 0];
    if (Asleep) {
      Asleep = false;
      return res;
    }
    if (action == 'a' || action == 'A') {
      if (CheckLeft(lvl, 1)) {
        PosX--;
        stats.DistWalked++;
      } else
        res = Attack(lvl, PosX - 1, PosY);
    }
    if (action == 'd' || action == 'D') {
      if (CheckRight(lvl, 1)) {
        PosX++;
        stats.DistWalked++;
      } else
        res = Attack(lvl, PosX + 1, PosY);
    }
    if (action == 'w' || action == 'W') {
      if (CheckUp(lvl, 1)) {
        PosY--;
        stats.DistWalked++;
      } else
        res = Attack(lvl, PosX, PosY - 1);
    }
    if (action == 's' || action == 'S') {
      if (CheckDown(lvl, 1)) {
        PosY++;
        stats.DistWalked++;
      } else
        res = Attack(lvl, PosX, PosY + 1);
    }
    return res;
  }

  public void ProcessEffects() {
    if (EffCount > 0)
      EffCount--;
    else if (Effect != "") {
      if (Effect == "Health") {
        Hp_max = Hp_max - currPotion.Value > 0 ? Hp_max - currPotion.Value : 1;
        Hp = Hp - currPotion.Value > 0 ? Hp - currPotion.Value : 1;
      } else if (Effect == "Strength")
        Str -= currPotion.Value;
      else if (Effect == "Agility")
        Agl -= currPotion.Value;
      Effect = "";
    }
  }

  public (bool, int) ProcessDamage(int damage, string type) {
    // successful vampire attack
    if (damage > 0 && type == "v")
      Hp_max -= 2;
    else if (damage > 0 && type == "s") {
      // successful snake attack
      Random rnd = new();
      if (rnd.Next(1, 4) == 1)
        Asleep = true;
    }
    Hp -= damage;
    if (Hp < 0)
      Hp = 0;
    if (Hp == 0)
      return (true, damage);
    else
      return (false, damage);
  }

  public List<int> Attack(Level lvl, int targetX, int targetY) {
    List<int> res = [0, 0];
    if (lvl.field[targetY, targetX] < Level.enemyCode ||
        lvl.field[targetY, targetX] >= Level.itemCode)
      return res;
    int pos = lvl.field[targetY, targetX] - Level.enemyCode;
    int enemyAgl = lvl.enemies[pos].Agl, chance;
    Random rnd = new();
    if (enemyAgl < Agl)
      chance = 40;
    else if (enemyAgl == Agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance)
      res[1] = Str;  // hit
    else
      res[1] = 0;  // miss
    res[0] = lvl.field[targetY, targetX];
    return res;
  }

  public int CollectItem(Level lvl, int targetX, int targetY) {
    if (lvl.field[targetY, targetX] < Level.itemCode)
      return -1;
    int pos = lvl.field[targetY, targetX] - Level.itemCode;
    var i = lvl.items[pos];
    if (!i.Active)
      return -1;
    if (i is Treasure)
      AddTreasure(i.Value);
    else if (!backpack.AddItem(i))
      return -1;
    i.Active = false;
    i.Symbol = "";
    return pos;
  }

  public void UseItem(Item item, Statistics stats) {
    if (item.Subtype == "Health" && item is Food)
      Hp = (Hp + item.Value > Hp_max) ? Hp_max : Hp + item.Value;
    else if (item.Subtype == "Health") {
      Hp_max += item.Value;
      Hp += item.Value;
    } else if (item.Subtype == "Strength")
      Str += item.Value;
    else if (item.Subtype == "Agility")
      Agl += item.Value;
    if (item is Weapon) {
      if (currWeapon.Equipped)
        Str -= currWeapon.Value;
      currWeapon.Equipped = true;
      currWeapon.Name = item.Name;
      currWeapon.Value = item.Value;
    }
    if (item is Potion p) {
      EffCount = 0;
      ProcessEffects();
      Effect = p.Subtype;
      EffCount = p.EffectLen;
      currPotion = p;
    }
    backpack.RemoveItem(item, stats);
  }

  public bool UseKey(Key key, Level lvl) {
    bool ok = true;
    int door;
    if (key.Value == (int)Colors.BLUE)
      door = (int)MapCellStates.DOOR_BLUE;
    else if (key.Value == (int)Colors.RED)
      door = (int)MapCellStates.DOOR_RED;
    else
      door = (int)MapCellStates.DOOR_GREEN;
    if (lvl.field[PosY, PosX + 1] == door || lvl.field[PosY, PosX - 1] == door ||
        lvl.field[PosY - 1, PosX] == door || lvl.field[PosY + 1, PosX] == door) {
      // found door
      foreach (var d in lvl.doors) {
        if (d.color == key.Value)
          d.lockState = (int)DoorLockState.OPEN;
      }
      backpack.RemoveItem(key, new Statistics());
    } else
      ok = false;
    return ok;
  }

  public void RemoveCurrWeapon() {
    currWeapon.Equipped = false;
    var w = new Weapon { Name = currWeapon.Name, Value = currWeapon.Value };
    backpack.AddItem(w);
    Str -= currWeapon.Value;
  }

  public void AddTreasure(int num) {
    backpack.AddTreasure(num);
  }

  public int GetTreasure() {
    return backpack.GetTreasure();
  }
}