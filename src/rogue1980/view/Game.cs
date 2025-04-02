namespace View.Game;

using Mindmagma.Curses;

using Domain.Player;
using Domain.Enemies;
using Domain.Level;

class Game {
  public const int X_BORDER = 5, Y_BORDER = 1, MSG_START = 3;
  public bool isOver = false;
  public (bool, int) killEnemy = (false, 0);

  public Level lvl = new Level();
  public Player player = new Player(34, 14);
  public Queue<string> messages = new Queue<string>();
  public List<int> attackResult = new List<int>();

  public Game() {
    // generate some enemies
    lvl.SpawnEnemy("z", 12, 5);
    lvl.SpawnEnemy("s", 6, 10);
    lvl.SpawnEnemy("m", 57, 14);
    lvl.SpawnEnemy("v", 30, 12);

    NCurses.StartColor();
    NCurses.InitPair((int)Colors.BLUE, CursesColor.BLUE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.WHITE, CursesColor.WHITE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.RED, CursesColor.RED, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.YELLOW, CursesColor.YELLOW, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.GREEN, CursesColor.GREEN, CursesColor.BLACK);
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
    return "";
  }

  public void ListInventory() {
    NCurses.AttributeSet(NCurses.ColorPair(4) | CursesAttribute.NORMAL);
    NCurses.MoveAddString(Level.ROWS + MSG_START, X_BORDER + 1,
                          String.Format("Treasure: {0} Coins", player.backpack.treasure));
  }

  public void DrawScene() {
    DrawField();
    DrawPlayer();
    DrawEnemies();
  }

  public void DrawMessages() {
    NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
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
    int count = 0;
    foreach (string msg in messages) {
      NCurses.MoveAddString(Level.ROWS + MSG_START + count, X_BORDER + 1, msg);
      count++;
    }
  }

  public void DrawField() {
    NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
    // field borders
    NCurses.MoveAddString(0, X_BORDER,
                          " ____________________________________________________________________ ");
    NCurses.MoveAddString(Level.ROWS, X_BORDER,
                          "|____________________________________________________________________|");
    for (int i = 1; i < Level.ROWS; i++) {
      NCurses.MoveAddString(
          i, X_BORDER, "|                                                                    |");
    }
    // room(s)
    for (int i = 0; i < Level.ROWS; i++) {
      for (int j = 0; j < Level.COLS; j++) {
        if (lvl.field[i, j] == (int)CellStates.WALL) {
          NCurses.MoveAddString(i + Y_BORDER, j + X_BORDER, ".");
        }
      }
    }
    // status bar
    NCurses.MoveAddString(
        Level.ROWS + 1, X_BORDER + 1,
        String.Format("Level: {0}(21), Health: {1}({2}), Strength: {3}, Agility: {4}", player.lvl,
                      player.hp, player.hp_max, player.str, player.agl));
  }

  public void DrawPlayer() {
    NCurses.AttributeSet(NCurses.ColorPair(player.color) | CursesAttribute.BOLD);
    NCurses.MoveAddString(player.y + Y_BORDER, player.x + X_BORDER, player.symbol);
  }

  public void DrawEnemies() {
    NCurses.AttributeSet(CursesAttribute.BOLD);
    foreach (var z in lvl.zombies) {
      NCurses.AttributeSet(NCurses.ColorPair(z.color));
      NCurses.MoveAddString(z.y + Y_BORDER, z.x + X_BORDER, z.symbol);
    }
    foreach (var v in lvl.vampires) {
      NCurses.AttributeSet(NCurses.ColorPair(v.color));
      NCurses.MoveAddString(v.y + Y_BORDER, v.x + X_BORDER, v.symbol);
    }
    foreach (var o in lvl.ogres) {
      NCurses.AttributeSet(NCurses.ColorPair(o.color));
      NCurses.MoveAddString(o.y + Y_BORDER, o.x + X_BORDER, o.symbol);
    }
    foreach (var g in lvl.ghosts) {
      NCurses.AttributeSet(NCurses.ColorPair(g.color));
      NCurses.MoveAddString(g.y + Y_BORDER, g.x + X_BORDER, g.symbol);
    }
    foreach (var s in lvl.snakes) {
      NCurses.AttributeSet(NCurses.ColorPair(s.color));
      NCurses.MoveAddString(s.y + Y_BORDER, s.x + X_BORDER, s.symbol);
    }
    foreach (var m in lvl.mimics) {
      NCurses.AttributeSet(NCurses.ColorPair(m.color));
      NCurses.MoveAddString(m.y + Y_BORDER, m.x + X_BORDER, m.symbol);
    }
  }

  public void GameEndMessage(string killer) {
    NCurses.AttributeSet(NCurses.ColorPair(3) | CursesAttribute.NORMAL);
    string msg = string.Format("You were defeated by {0}!", killer);
    NCurses.MoveAddString(Level.ROWS + MSG_START + messages.Count(), X_BORDER + 1, msg);
  }
}
