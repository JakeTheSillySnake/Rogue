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
    DrawEnemies();
    DrawItems();
    DrawPlayer();
  }

  public void ProcessKeys(int action, Game game) {
    player = game.player;
    lvl = game.lvl;
    messages = game.messages;
    NCurses.Erase();
    if (action == 'i' || action == 'I') {
      DrawScene();
      ListInventory();
    } else {
      var killer = game.UpdateGame(action);
      DrawScene();
      DrawMessages();
      if (game.isOver) {
        GameEndMessage(killer);
      }
    }
    NCurses.Refresh();
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
        string.Format("Level: {0}(21), Health: {1}({2}), Strength: {3}, Agility: {4}", player.lvl,
                      player.hp, player.hp_max, player.str, player.agl));
  }

  public void DrawPlayer() {
    NCurses.AttributeSet(NCurses.ColorPair(player.color) | CursesAttribute.BOLD);
    NCurses.MoveAddString(player.y + Y_BORDER, player.x + X_BORDER, player.symbol);
  }

  public void DrawEnemies() {
    NCurses.AttributeSet(CursesAttribute.BOLD);
    foreach (var e in lvl.enemies) {
      NCurses.AttributeSet(NCurses.ColorPair(e.color));
      NCurses.MoveAddString(e.y + Y_BORDER, e.x + X_BORDER, e.symbol);
    }
  }

  public void DrawItems() {
    NCurses.AttributeSet(NCurses.ColorPair(2));
    foreach (var i in lvl.items) {
      if (i is Treasure)
        NCurses.AttributeSet(NCurses.ColorPair(4));
      NCurses.MoveAddString(i.y + Y_BORDER, i.x + X_BORDER, i.symbol);
    }
  }

  public void ListInventory() {
    NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
    Queue<string> items = new();
    items.Enqueue(string.Format("Treasure: {0} Coins", player.GetTreasure()));
    items.Enqueue(string.Format("Potions: {0}", player.backpack.potions.Count));
    items.Enqueue(string.Format("Scrolls: {0}", player.backpack.scrolls.Count));
    items.Enqueue(string.Format("Food: {0}", player.backpack.food.Count));
    items.Enqueue(string.Format("Weapons: {0}", player.backpack.weapons.Count));
    int cnt = 0;
    foreach (var i in items) {
      NCurses.MoveAddString(Level.ROWS + MSG_START + cnt, X_BORDER + 1, i);
      cnt++;
    }
  }

  public void GameEndMessage(string killer) {
    NCurses.AttributeSet(NCurses.ColorPair(3) | CursesAttribute.NORMAL);
    string msg = string.Format("You were defeated by {0}!", killer);
    NCurses.MoveAddString(Level.ROWS + MSG_START + messages.Count, X_BORDER + 1, msg);
  }
}