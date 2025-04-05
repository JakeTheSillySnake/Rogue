namespace rogue.View;

using rogue.Domain;

class Game {
  public bool isOver = false, killEnemy = false;
    
  public Level lvl = new();
  public Player player = new(34, 14);
  public Queue<string> messages = new();
  public List<int> attackResult = [];

  public Game() {
    // generate some enemies
    lvl.SpawnEnemy((int)Enemies.ZOMBIE, 12, 5);
    // lvl.SpawnEnemy((int)Enemies.SNAKE, 6, 10);
    lvl.SpawnEnemy((int)Enemies.MIMIC, 57, 14);
    // lvl.SpawnEnemy((int)Enemies.VAMPIRE, 30, 12);

    // generate some items
    lvl.SpawnItem((int)Items.POTION, 40, 14);
    lvl.SpawnItem((int)Items.SCROLL, 50, 10);
    lvl.SpawnItem((int)Items.FOOD, 6, 12);
    lvl.SpawnItem((int)Items.WEAPON, 30, 12);
    lvl.SpawnItem((int)Items.WEAPON, 20, 10);
  }

  public string UpdateGame(int action) {
    messages.Clear();
    // damage to enemy
    attackResult = player.Move(action, lvl);
    killEnemy = lvl.ProcessDamage(attackResult);
    ProcessItemMessages();

    // damage to player
    string attacker = ProcessDamageMessages();
    ProcessAttackMessages();
    return attacker;
  }

  public void ProcessItemMessages() {
    int pos = player.CollectItem(lvl, player.x, player.y);
    if (pos < 0)
      return;
    var i = lvl.items[pos];
    string item = "";
    if (i is Treasure)
      item = string.Format("{0} Coins", i.value);
    else if (i is Potion)
      item = string.Format("Potion of {0}", i.name);
    else if (i is Scroll)
      item = string.Format("Scroll of {0}", i.name);
    else if (i is Food)
      item = string.Format("{0}", i.name);
    else if (i is Weapon)
      item = string.Format("{0}", i.name);
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
