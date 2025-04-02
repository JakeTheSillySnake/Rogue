namespace View.Game;

using Mindmagma.Curses;

using Domain.Player;
using Domain.Enemies;
using Domain.Level;
using rogue1980.domain;

class Game {
  public const int X_BORDER = 5, Y_BORDER = 1;
  public bool isOver = false;
  public Level lvl = new Level();
  public Player player = new Player(34, 14);

  public Game() {
    // generate enemies
    lvl.SpawnEnemy("z", 12, 5);
    lvl.SpawnEnemy("s", 6, 10);
    lvl.SpawnEnemy("g", 57, 14);

    NCurses.StartColor();
    NCurses.InitPair((int)Colors.BLUE, CursesColor.BLUE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.WHITE, CursesColor.WHITE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.RED, CursesColor.RED, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.YELLOW, CursesColor.YELLOW, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.GREEN, CursesColor.GREEN, CursesColor.BLACK);
  }

  public string UpdateGame() {
    // damage from enemies
    foreach (var z in lvl.zombies) {
      isOver = player.ProcessDamage(z.Act(lvl, player), z.symbol);
      lvl.UpdateField();
      if (isOver) return "Zombie";
    }
    foreach (var v in lvl.vampires) {
      isOver = player.ProcessDamage(v.Act(lvl, player), v.symbol);
      lvl.UpdateField();
      if (isOver) return "Vampire";
    }
    foreach (var o in lvl.ogres) {
      isOver = player.ProcessDamage(o.Act(lvl, player), o.symbol);
      lvl.UpdateField();
      if (isOver) return "Ogre";
    }
    foreach (var g in lvl.ghosts) {
      isOver = player.ProcessDamage(g.Act(lvl, player), g.symbol);
      lvl.UpdateField();
      if (isOver) return "Ghost";
    }
    foreach (var s in lvl.snakes) {
      isOver = player.ProcessDamage(s.Act(lvl, player), s.symbol);
      lvl.UpdateField();
      if (isOver) return "Snake-Wizard";
    }
    foreach (var m in lvl.mimics) {
      isOver = player.ProcessDamage(m.Act(lvl, player), m.symbol);
      lvl.UpdateField();
      if (isOver) return "Mimic";
    }
    return "";
  }

  public void DrawMessages(List<int> attackResult, bool dead) {
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
      NCurses.MoveAddString(Level.ROWS + 3, X_BORDER + 1,
                            String.Format("You dealt {0} damage to {1}", attackResult[1], enemy));
      if (dead)
        NCurses.MoveAddString(Level.ROWS + 4, X_BORDER + 1,
                              String.Format("You defeated {0}", enemy));
    } else if (enemy != "") {
      NCurses.MoveAddString(Level.ROWS + 3, X_BORDER + 1,
                            String.Format("You tried to hit {0} but missed", enemy));
    } 
    if (player.asleep) {
      NCurses.MoveAddString(Level.ROWS + 4, X_BORDER + 1, "You were stunned by Snake-Wizard");
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
    NCurses.MoveAddString(Level.ROWS + 5, X_BORDER + 1,
                            String.Format("You were defeated by {0}!", killer));
  }
}
