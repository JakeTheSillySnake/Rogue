namespace rogue.View;

using Mindmagma.Curses;
using rogue.Domain.LevelMap;
using rogue.Domain.Items;
using rogue.Data;

enum StartActions { Continue = 1, New, Stats }

class Scene {
  public const int X_BORDER = 1, Y_BORDER = 1, MSG_START = 3;
  public Game game = new();
  public Render render = new();

  public Scene() {
    NCurses.StartColor();
    NCurses.InitPair((int)Colors.BLUE, CursesColor.BLUE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.WHITE, CursesColor.WHITE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.RED, CursesColor.RED, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.YELLOW, CursesColor.YELLOW, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.GREEN, CursesColor.GREEN, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.MAGENTA, CursesColor.MAGENTA, CursesColor.BLACK);
  }

  public void DrawScene() {
    DrawField();
    DrawItems();
    DrawEnemies();
    DrawPlayer();
  }

  public void Start() {
    int c = StartMenu();
    if (c == (int)StartActions.Continue) {
      // load prev session
      if (!game.LoadSession()) {
        NCurses.Erase();
        NCurses.AttributeSet(NCurses.ColorPair(3));
        NCurses.MoveAddString(5, 23, "Couldn't load session. Press any key to exit.");
        NCurses.GetChar();
        return;
      }
    }
    if (c <= (int)StartActions.New) {
      // game loop
      while (c != 'Q' && c != 'q' && !game.isOver) {
        ProcessKeys(c);
        c = NCurses.GetChar();
      }
    } else if (c == (int)StartActions.Stats) {
      // load leaderboard stats from JSON
      LoadStats();
      NCurses.GetChar();
    }
  }

  public int StartMenu() {
    Queue<string> msg = new();
    msg.Enqueue("Continue last session");
    msg.Enqueue("Start new game");
    msg.Enqueue("See leaderboard statistics");
    msg.Enqueue("Exit");
    int count = 1, c = 0;
    NCurses.AttributeSet(NCurses.ColorPair(5));
    NCurses.MoveAddString(5, 23, "Welcome to Rogue! Please choose action (0-4):");
    NCurses.AttributeSet(NCurses.ColorPair(2));
    foreach (string m in msg) {
      NCurses.MoveAddString(10 + count, 30, string.Format("{0}) {1}", count, m));
      count++;
    }
    while (c - '0' < 1 || c - '0' > msg.Count) c = NCurses.GetChar();
    return c - '0';
  }

  public void LoadStats() {
    NCurses.Erase();
    Leaderboard leaderboard = new();
    var leadStats = leaderboard.LoadStats();
    if (leadStats.Count > 0) {
      NCurses.AttributeSet(NCurses.ColorPair(5));
      NCurses.MoveAddString(
          1, 1,
          "Treasure | LVL | Kills | Hits Dealt | Hits Received | Food | Potions | Scrolls | Distance");
    } else {
      NCurses.AttributeSet(NCurses.ColorPair(4));
      NCurses.MoveAddString(5, 23, "Nothing to display. Press any key to exit.");
    }
    NCurses.AttributeSet(NCurses.ColorPair(2));
    int count = 0;
    foreach (var stat in leadStats) {
      NCurses.MoveAddString(
          1 + count, 1,
          string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8}", stat.Treasure,
                        stat.Lvl, stat.Kills, stat.HitsDealt, stat.HitsReceived, stat.Food,
                        stat.Potions, stat.Scrolls, stat.DistWalked));
    }
  }

  public void ProcessKeys(int action) {
    List<Item> items = [];
    string killer = "";
    NCurses.Erase();
    char[] playerActions = ['a', 'A', 's', 'S', 'w', 'W', 'd', 'D'];
    char[] menuActions = ['i', 'I', 'h', 'H', 'j', 'J', 'k', 'K', 'l', 'L', 'e', 'E', 'x', 'X'];
    foreach (var act in playerActions) {
      if (action == act) {
        killer = game.UpdateGame(action);
        DrawMessages();
      }
    }
    DrawScene();
    int type = -1;
    if (action == 'i' || action == 'I') {
      game.msg.ListInventory(game.player);
    } else if (action == 'h' || action == 'H') {
      items.AddRange(game.player.backpack.weapons);
      type = (int)Items.WEAPON;
      game.msg.ListItems(game.player, items, type, "Weapon");
    } else if (action == 'j' || action == 'J') {
      items.AddRange(game.player.backpack.food);
      type = (int)Items.FOOD;
      game.msg.ListItems(game.player, items, type, "Food");
    } else if (action == 'k' || action == 'K') {
      items.AddRange(game.player.backpack.potions);
      type = (int)Items.POTION;
      game.msg.ListItems(game.player, items, type, "Potion");
    } else if (action == 'e' || action == 'E') {
      items.AddRange(game.player.backpack.scrolls);
      type = (int)Items.SCROLL;
      game.msg.ListItems(game.player, items, type, "Scroll");
    } else if (action == 'l' || action == 'L') {
      items.AddRange(game.player.backpack.keys);
      type = (int)Items.KEY;
      game.msg.ListItems(game.player, items, type, "Key");
    } else if (action == 'x' || action == 'X') {
      game.msg.DrawStats(game.stats);
    }
    foreach (var act in menuActions) {
      if (action == act)
        DrawMessages();
    }
    if (game.isOver)
      GameEndMessage(killer);
    NCurses.Refresh();
    if (type != -1)
      ItemUsedMessage(items, game, type);
  }

  public void ItemUsedMessage(List<Item> items, Game game, int type) {
    if (items.Count == 0 && type != (int)Items.WEAPON)
      return;
    if (items.Count == 0 && type == (int)Items.WEAPON && !game.player.currWeapon.Equipped)
      return;
    int choice = SelectItem(items, type);
    NCurses.Erase();
    DrawScene();
    if (choice != CursesKey.ESC) {
      string msg;
      if (choice == '0') {
        msg = string.Format("You put {0} away.", game.player.currWeapon.Name);
        game.RemoveCurrWeapon();
      } else {
        Item i = items[choice - '0' - 1];
        string name, action;
        if (i is Weapon || i is Food)
          name = i.Name;
        else
          name = string.Format("{0} of {1}", i.Type, i.Subtype);
        if (i is Weapon)
          action = "equipped";
        else
          action = "used";
        bool ok = game.UseItem(i);
        if (ok && i is Key)
          msg = string.Format("You unlocked {0} Door.", i.Subtype);
        else if (ok)
          msg = string.Format("You {3} {0} (+{1} {2}).", name, i.Value, i.Subtype, action);
        else if (i is Weapon)
          msg = "You can't change weapons here.";
        else
          msg = "Can't use this here.";
      }
      NCurses.AttributeSet(NCurses.ColorPair(5) | CursesAttribute.NORMAL);
      NCurses.MoveAddString(Level.ROWS + MSG_START, X_BORDER + 1, msg);
      NCurses.Refresh();
    }
  }

  public void DrawMessages() {
    NCurses.AttributeSet(NCurses.ColorPair(4) | CursesAttribute.NORMAL);
    int count = 0;
    foreach (string msg in game.msg.messages) {
      NCurses.MoveAddString(Level.ROWS + MSG_START + count, X_BORDER + 1, msg);
      count++;
    }
  }

  public void DrawField() {
    DrawBorders();
    DrawLevelMap();
    DrawStatusBar();
  }

  public void DrawBorders() {
    NCurses.AttributeSet(NCurses.ColorPair(2));
    for (int i = 1; i < Level.COLS - 1; i++) NCurses.MoveAddChar(Y_BORDER, X_BORDER + i, '_');
    for (int i = 0; i < Level.COLS; i++)
      NCurses.MoveAddChar(Level.ROWS + Y_BORDER - 1, X_BORDER + i, '_');
    for (int i = 1; i < Level.ROWS; i++) {
      NCurses.MoveAddChar(Y_BORDER + i, X_BORDER, '|');
      NCurses.MoveAddChar(Y_BORDER + i, Level.COLS, '|');
    }
  }

  public void DrawLevelMap() {
    var fieldMask = render.CreateLevelMask(game.lvl, game.player);
    // exit point
    List<int> endPos = game.lvl.GetEndPos();
    if (render.fieldMask[endPos[0], endPos[1]] == (int)MapCellStates.EXIT)
      NCurses.MoveAddString(endPos[0] + Y_BORDER, endPos[1] + X_BORDER, "E");

    NCurses.AttributeSet(NCurses.ColorPair(6));
    for (int y = 0; y < Level.ROWS; y++) {
      for (int x = 0; x < Level.COLS; x++) {
        if (fieldMask[y, x] == (int)MapCellStates.WALL) {
          NCurses.MoveAddString(y + Y_BORDER, x + X_BORDER, "#");
        }
        if (fieldMask[y, x] == (int)MapCellStates.CORRIDOR ||
            fieldMask[y, x] == (int)MapCellStates.EMPTY ||
            fieldMask[y, x] == (int)MapCellStates.ENTER) {
          if (y != game.player.PosY || x != game.player.PosX)
            NCurses.MoveAddString(y + Y_BORDER, x + X_BORDER, ".");
        }
        if (fieldMask[y, x] == (int)MapCellStates.DOOR) {
          NCurses.MoveAddString(y + Y_BORDER, x + X_BORDER, "/");
        }
      }
    }
  }

  public void DrawStatusBar() {
    // controls menu
    NCurses.AttributeSet(NCurses.ColorPair(2));
    NCurses.MoveAddString(Y_BORDER - 1, X_BORDER + 1,
                          "Player: WASD, Inventory: I, Quit: Q, Current Stats: X, Level Exit: E");
    // status bar
    string effect = "None";
    if (game.player.Effect != "")
      effect = string.Format("{0} ({1})", game.player.Effect, game.player.EffCount);
    NCurses.MoveAddString(
        Level.ROWS + Y_BORDER, X_BORDER + 1,
        string.Format("LVL: {0}(21), Hp: {1}({2}), STR: {3}, Agl: {4}, EFFECTS: {5}",
                      game.player.Lvl, game.player.Hp, game.player.Hp_max, game.player.Str,
                      game.player.Agl, effect));
  }

  public void DrawPlayer() {
    NCurses.AttributeSet(NCurses.ColorPair(game.player.Color) | CursesAttribute.BOLD);
    NCurses.MoveAddString(game.player.PosY + Y_BORDER, game.player.PosX + X_BORDER,
                          game.player.Symbol);
  }

  public void DrawEnemies() {
    NCurses.AttributeSet(CursesAttribute.BOLD);
    foreach (var e in game.lvl.enemies) {
      NCurses.AttributeSet(NCurses.ColorPair(e.Color));
      if (render.fieldMask[e.PosY, e.PosX] >= Level.enemyCode)
        NCurses.MoveAddString(e.PosY + Y_BORDER, e.PosX + X_BORDER, e.Symbol);
    }
  }

  public void DrawItems() {
    foreach (var i in game.lvl.items) {
      NCurses.AttributeSet(NCurses.ColorPair(2));
      if (i is Treasure)
        NCurses.AttributeSet(NCurses.ColorPair(4));
      if (i is Key k)
        NCurses.AttributeSet(NCurses.ColorPair(k.Value));
      if (render.fieldMask[i.PosY, i.PosX] >= Level.itemCode)
        NCurses.MoveAddString(i.PosY + Y_BORDER, i.PosX + X_BORDER, i.Symbol);
    }
    foreach (var d in game.lvl.doors) {
      NCurses.AttributeSet(NCurses.ColorPair(d.color));
      if (render.fieldMask[d.posY, d.posX] != (int)MapCellStates.BUSY)
        NCurses.MoveAddString(d.posY + Y_BORDER, d.posX + X_BORDER, "/");
    }
  }

  public int SelectItem(List<Item> items, int type) {
    int c = 0;
    int min =
        (items.Count < 9 && type == (int)Items.WEAPON && game.player.currWeapon.Equipped) ? 0 : 1;
    while (c != CursesKey.ESC) {
      c = NCurses.GetChar();
      if (c - '0' >= min && c - '0' <= items.Count)
        break;
    }
    return c;
  }

  public void GameEndMessage(string killer) {
    string msg;
    if (killer == "") {
      NCurses.AttributeSet(NCurses.ColorPair(5) | CursesAttribute.NORMAL);
      msg = string.Format("You completed the dungeon! Total score: {0}. Press any key to exit.",
                          game.player.GetTreasure());
    } else {
      NCurses.AttributeSet(NCurses.ColorPair(3) | CursesAttribute.NORMAL);
      msg = string.Format("You were defeated by {0}! Press any key to exit.", killer);
    }
    NCurses.MoveAddString(Level.ROWS + MSG_START + game.msg.messages.Count, X_BORDER + 1, msg);
  }
}