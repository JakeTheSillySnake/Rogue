namespace rogue.View;

using Mindmagma.Curses;
using rogue.Domain;

class Scene {
  public const int X_BORDER = 5, Y_BORDER = 1, MSG_START = 3;
  public Level lvl = new();
  public Player player = new(0, 0);
  public Queue<string> messages = new();

  public Scene() {
    NCurses.StartColor();
    NCurses.InitPair((int)Colors.BLUE, CursesColor.BLUE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.WHITE, CursesColor.WHITE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.RED, CursesColor.RED, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.YELLOW, CursesColor.YELLOW, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.GREEN, CursesColor.GREEN, CursesColor.BLACK);
  }

  public void DrawScene() {
    DrawField();
    DrawPlayer();
    DrawEnemies();
  }

  public void UploadGame(Game game) {
    player = game.player;
    lvl = game.lvl;
    messages = game.messages;
  }

  public void DrawMessages() {
    NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
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

  public void ListInventory() {
    NCurses.AttributeSet(NCurses.ColorPair(4) | CursesAttribute.NORMAL);
    NCurses.MoveAddString(Level.ROWS + MSG_START, X_BORDER + 1,
                          string.Format("Treasure: {0} Coins", player.backpack.treasure));
  }

  public void GameEndMessage(string killer) {
    NCurses.AttributeSet(NCurses.ColorPair(3) | CursesAttribute.NORMAL);
    string msg = string.Format("You were defeated by {0}!", killer);
    NCurses.MoveAddString(Level.ROWS + MSG_START + messages.Count(), X_BORDER + 1, msg);
  }
}