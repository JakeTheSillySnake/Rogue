namespace rogue.View;

using Mindmagma.Curses;
using rogue.Domain.LevelMap;
using rogue.Domain;

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
      // load prev session from JSON
      // game.LoadSession() maybe
    }
    if (c <= (int)StartActions.New) {
      // game loop
      while (c != 'Q' && c != 'q' && !game.isOver) {
        ProcessKeys(c);
        c = NCurses.GetChar();
      }
    } else if (c == (int)StartActions.Stats) {
      // load leaderboard stats from JSON
      // LoadStats()
    }
  }

  public int StartMenu() {
    game.messages.Enqueue("Continue last session");
    game.messages.Enqueue("Start new game");
    game.messages.Enqueue("See leaderboard statistics");
    game.messages.Enqueue("Exit");
    int count = 1, c = 0;
    NCurses.AttributeSet(NCurses.ColorPair(5));
    NCurses.MoveAddString(5, 23, "Welcome to Rogue! Please choose action (0-4):");
    NCurses.AttributeSet(NCurses.ColorPair(2));
    foreach (string msg in game.messages) {
      NCurses.MoveAddString(10 + count, 30, string.Format("{0}) {1}", count, msg));
      count++;
    }
    while (c - '0' < 1 || c - '0' > game.messages.Count) c = NCurses.GetChar();
    return c - '0';
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
      ListInventory();
    } else if (action == 'h' || action == 'H') {
      items.AddRange(game.player.backpack.weapons);
      type = (int)Items.WEAPON;
      ListItems(items, type, "Weapon");
    } else if (action == 'j' || action == 'J') {
      items.AddRange(game.player.backpack.food);
      type = (int)Items.FOOD;
      ListItems(items, type, "Food");
    } else if (action == 'k' || action == 'K') {
      items.AddRange(game.player.backpack.potions);
      type = (int)Items.POTION;
      ListItems(items, type, "Potion");
    } else if (action == 'e' || action == 'E') {
      items.AddRange(game.player.backpack.scrolls);
      type = (int)Items.SCROLL;
      ListItems(items, type, "Scroll");
    } else if (action == 'l' || action == 'L') {
      items.AddRange(game.player.backpack.keys);
      type = (int)Items.KEY;
      ListItems(items, type, "Key");
    } else if (action == 'x' || action == 'X') {
      DrawStats();
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
    if (items.Count == 0 && type == (int)Items.WEAPON && !game.player.currWeapon.equipped)
      return;
    int choice = SelectItem(items, type);
    NCurses.Erase();
    DrawScene();
    if (choice != CursesKey.ESC) {
      string msg;
      if (choice == '0') {
        msg = string.Format("You put {0} away.", game.player.currWeapon.name);
        game.RemoveCurrWeapon();
      } else {
        Item i = items[choice - '0' - 1];
        string name, action;
        if (i is Weapon || i is Food)
          name = i.name;
        else
          name = string.Format("{0} of {1}", i.type, i.subtype);
        if (i is Weapon)
          action = "equipped";
        else
          action = "used";
        bool ok = game.UseItem(i);
        if (ok && i is Key)
          msg = string.Format("You unlocked {0} Door.", i.subtype);
        else if (ok)
          msg = string.Format("You {3} {0} (+{1} {2}).", name, i.value, i.subtype, action);
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
    foreach (string msg in game.messages) {
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
          if (y != game.player.y || x != game.player.x)
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
    if (game.player.effect != "")
      effect = string.Format("{0} ({1})", game.player.effect, game.player.effCount);
    NCurses.MoveAddString(
        Level.ROWS + Y_BORDER, X_BORDER + 1,
        string.Format("LVL: {0}(21), HP: {1}({2}), STR: {3}, AGL: {4}, EFFECTS: {5}",
                      game.player.lvl, game.player.hp, game.player.hp_max, game.player.str,
                      game.player.agl, effect));
  }

  public void DrawPlayer() {
    NCurses.AttributeSet(NCurses.ColorPair(game.player.color) | CursesAttribute.BOLD);
    NCurses.MoveAddString(game.player.y + Y_BORDER, game.player.x + X_BORDER, game.player.symbol);
  }

  public void DrawEnemies() {
    NCurses.AttributeSet(CursesAttribute.BOLD);
    foreach (var e in game.lvl.enemies) {
      NCurses.AttributeSet(NCurses.ColorPair(e.color));
      if (render.fieldMask[e.y, e.x] >= Level.enemyCode)
        NCurses.MoveAddString(e.y + Y_BORDER, e.x + X_BORDER, e.symbol);
    }
  }

  public void DrawItems() {
    foreach (var i in game.lvl.items) {
      NCurses.AttributeSet(NCurses.ColorPair(2));
      if (i is Treasure)
        NCurses.AttributeSet(NCurses.ColorPair(4));
      if (i is Key k)
        NCurses.AttributeSet(NCurses.ColorPair(k.value));
      if (render.fieldMask[i.y, i.x] >= Level.itemCode)
        NCurses.MoveAddString(i.y + Y_BORDER, i.x + X_BORDER, i.symbol);
    }
    foreach (var d in game.lvl.doors) {
      NCurses.AttributeSet(NCurses.ColorPair(d.color));
      if (render.fieldMask[d.posY, d.posX] != (int)MapCellStates.BUSY)
        NCurses.MoveAddString(d.posY + Y_BORDER, d.posX + X_BORDER, "/");
    }
  }

  public void DrawStats() {
    game.messages.Clear();
    game.messages.Enqueue(
        string.Format("Level: {0}, treasure collected: {1}", game.stats.lvl, game.stats.treasure));
    game.messages.Enqueue(string.Format("Used potions: {0}, scrolls: {1}, food: {2}",
                                        game.stats.potions, game.stats.scrolls, game.stats.food));
    game.messages.Enqueue(string.Format("Mobs killed: {0}, hits dealt: {1}, hits received: {2}",
                                        game.stats.kills, game.stats.hitsDealt,
                                        game.stats.hitsReceived));
    game.messages.Enqueue(string.Format("Distance traveled: {0}", game.stats.distWalked));
  }

  public void ListInventory() {
    game.messages.Clear();
    game.messages.Enqueue(string.Format("Treasure: {0} Coins", game.player.GetTreasure()));
    game.messages.Enqueue(
        string.Format("Potions: {0} (Press K to Use)", game.player.backpack.potions.Count));
    game.messages.Enqueue(
        string.Format("Scrolls: {0} (Press E to Use)", game.player.backpack.scrolls.Count));
    game.messages.Enqueue(
        string.Format("Food: {0} (Press J to Use)", game.player.backpack.food.Count));
    game.messages.Enqueue(
        string.Format("Weapons: {0} (Press H to Use)", game.player.backpack.weapons.Count));
    game.messages.Enqueue(
        string.Format("Keys: {0} (Press L to Use)", game.player.backpack.keys.Count));
  }

  public void ListItems(List<Item> items, int type, string stype) {
    game.messages.Clear();
    if (items.Count == 0 && type != (int)Items.WEAPON) {
      game.messages.Enqueue("There is nothing here.");
      return;
    }
    if (items.Count == 0 && type == (int)Items.WEAPON && !game.player.currWeapon.equipped) {
      game.messages.Enqueue("There is nothing here.");
      return;
    }
    int begin =
        (items.Count < 9 && type == (int)Items.WEAPON && game.player.currWeapon.equipped) ? 0 : 1;
    game.messages.Enqueue(string.Format("Choose {0} | ESC to return:", stype));
    if (items.Count < 9 && type == (int)Items.WEAPON && game.player.currWeapon.equipped) {
      game.messages.Enqueue("0. Unequip current weapon");
      begin++;
    }
    foreach (var i in items) {
      string name;
      if (i is Weapon || i is Food)
        name = i.name;
      else if (i is Key)
        name = string.Format("{0} Key", i.subtype);
      else
        name = string.Format("{0} of {1}", i.type, i.subtype);
      if (i is Key)
        game.messages.Enqueue(string.Format("{0}. {1}", begin, name));
      else
        game.messages.Enqueue(
            string.Format("{0}. {1} ({2} +{3})", begin, name, i.subtype, i.value));
      begin++;
    }
  }

  public int SelectItem(List<Item> items, int type) {
    int c = 0;
    int min =
        (items.Count < 9 && type == (int)Items.WEAPON && game.player.currWeapon.equipped) ? 0 : 1;
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
      msg = string.Format("You completed the dungeon! Total score: {0}. Press any key to exit",
                          game.player.GetTreasure());
    } else {
      NCurses.AttributeSet(NCurses.ColorPair(3) | CursesAttribute.NORMAL);
      msg = string.Format("You were defeated by {0}! Press any key to exit", killer);
    }
    NCurses.MoveAddString(Level.ROWS + MSG_START + game.messages.Count, X_BORDER + 1, msg);
  }
}