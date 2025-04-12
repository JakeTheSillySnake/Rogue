namespace rogue.View;

using rogue.Domain;
using rogue.Domain.LevelMap;

class Game {
  public bool isOver = false, killEnemy = false;
  private int _difficulty = 2;

  public Level lvl;
  public Player player;
  public Queue<string> messages = new();
  public List<int> attackResult = [];

  public Game() {
    lvl = new(_difficulty);
    var playerPos = lvl.GetStartPos();
    player = new(playerPos[1], playerPos[0]);
  }

  public string UpdateGame(int action) {
    messages.Clear();
    // check for level end
    List<int> endPos = lvl.GetEndPos();
    if (endPos[0] == player.y && endPos[1] == player.x) {
      if (player.lvl == 22)
        isOver = true;
      else 
        NextLevel();
      return "";
    }
    // damage to enemy
    attackResult = player.Move(action, lvl);
    killEnemy = lvl.ProcessDamage(attackResult, _difficulty);
    ProcessItemMessages();

    // damage to player
    string attacker = ProcessDamageMessages();
    ProcessAttackMessages();
    return attacker;
  }

  public void NextLevel() {
    // adjust difficulty
    player.lvl++;
    _difficulty = player.lvl / 2;
    if ((float)player.hp / player.hp_max <= 0.5)
      _difficulty = _difficulty > 1 ? _difficulty - 1 : 1;

    lvl = new(_difficulty);
    var playerPos = lvl.GetStartPos();
    player.InitCoords(playerPos[1], playerPos[0]);
  }

  public bool UseItem(Item item) {
    bool success = true;
    if (item is Weapon && player.currWeapon.equipped)
      success = lvl.DropWeapon(player);
    if (success)
      player.UseItem(item);
    // TODO: add check for keys vs doors
    return success;
  }

  public void RemoveCurrWeapon() {
    player.RemoveCurrWeapon();
  }

  public void ProcessItemMessages() {
    int pos = player.CollectItem(lvl, player.x, player.y);
    if (pos < 0)
      return;
    var i = lvl.items[pos];
    if (i is Weapon w)
      lvl.PickUpWeapon(w);
    string item = "";
    if (i is Treasure)
      item = string.Format("{0} Coins", i.value);
    else if (i is Potion || i is Scroll)
      item = string.Format("{0} of {1}", i.type, i.subtype);
    else if (i is Food || i is Weapon)
      item = string.Format("{0}", i.name);
    else if (i is Key)
      item = string.Format("{0} Key", i.subtype);
    messages.Enqueue(string.Format("You collected {0}!", item));
  }

  public string ProcessDamageMessages() {
    foreach (var e in lvl.enemies) {
      (bool, int)attack = (false, 0);
      string attacker = "";
      if (e is Zombie z) {
        attack = player.ProcessDamage(z.Act(lvl, player), z.symbol);
        attacker = "Zombie";
      } else if (e is Vampire v) {
        attack = player.ProcessDamage(v.Act(lvl, player), v.symbol);
        attacker = "Vampire";
      } else if (e is Ogre o) {
        attack = player.ProcessDamage(o.Act(lvl, player), o.symbol);
        attacker = "Ogre";
      } else if (e is Ghost g) {
        attack = player.ProcessDamage(g.Act(lvl, player), g.symbol);
        attacker = "Ghost";
      } else if (e is Snake s) {
        attack = player.ProcessDamage(s.Act(lvl, player), s.symbol);
        attacker = "Snake-Wizard";
      } else if (e is Mimic m) {
        attack = player.ProcessDamage(m.Act(lvl, player), m.symbol);
        attacker = "Mimic";
      }
      isOver = attack.Item1;
      lvl.UpdateField();
      if (attack.Item2 > 0)
        messages.Enqueue(string.Format("You were hit by {0}! (-{1} HP)", attacker, attack.Item2));
      if (isOver)
        return attacker;
    }
    return "";
  }

  public void ProcessAttackMessages() {
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
      if (killEnemy)
        messages.Enqueue(string.Format("You defeated {0}!", enemy));
    } else if (enemy != "")
      messages.Enqueue(string.Format("You tried to hit {0} but missed!", enemy));
    if (player.asleep)
      messages.Enqueue("You were stunned by Snake-Wizard!");
  }
}
