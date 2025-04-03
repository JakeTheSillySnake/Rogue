namespace rogue.View;

using rogue.Domain;

class Game {
  public bool isOver = false;
  public (bool, int) killEnemy = (false, 0);

  public Level lvl = new();
  public Player player = new(34, 14);
  public Queue<string> messages = new();
  public List<int> attackResult = [];

  public Game() {
    // generate some enemies
    lvl.SpawnEnemy("z", 12, 5);
    lvl.SpawnEnemy("s", 6, 10);
    lvl.SpawnEnemy("m", 57, 14);
    lvl.SpawnEnemy("v", 30, 12);
  }

  public string UpdateGame(int action) {
    messages.Clear();
    var enemies = new List<Enemy>(lvl.zombies);
    enemies.AddRange(lvl.vampires);
    enemies.AddRange(lvl.ogres);
    enemies.AddRange(lvl.ghosts);
    enemies.AddRange(lvl.snakes);
    enemies.AddRange(lvl.mimics);
    // damage to enemy
    attackResult = player.Move(action, lvl);
    killEnemy = lvl.ProcessDamage(attackResult);
    if (killEnemy.Item1)
      player.AddTreasure(killEnemy.Item2);
    // damage to player
    foreach (var e in enemies) {
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
    ProcessMessages();
    return "";
  }

  public void ProcessMessages() {
    int enemyType = attackResult[0] / 1000;
    string enemy = "";
    switch (enemyType) {
      case (int)CellStates.ZOMBIE:
        enemy = "Zombie";
        break;
      case (int)CellStates.VAMPIRE:
        enemy = "Vampire";
        break;
      case (int)CellStates.OGRE:
        enemy = "Ogre";
        break;
      case (int)CellStates.GHOST:
        enemy = "Ghost";
        break;
      case (int)CellStates.SNAKE:
        enemy = "Snake-Wizard";
        break;
      case (int)CellStates.MIMIC:
        enemy = "Mimic";
        break;
    }
    if (enemy != "" && attackResult[1] != 0) {
      messages.Enqueue(string.Format("You dealt {0} damage to {1}!", attackResult[1], enemy));
      if (killEnemy.Item1)
        messages.Enqueue(string.Format("You defeated {0}! (+{1} Coins)", enemy, killEnemy.Item2));
    } else if (enemy != "")
      messages.Enqueue(string.Format("You tried to hit {0} but missed!", enemy));
    if (player.asleep)
      messages.Enqueue("You were stunned by Snake-Wizard!");
  }
}
