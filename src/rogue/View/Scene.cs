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
    List<Item> items = [];
    string killer = "";
    NCurses.Erase();
    char[] playerActions = ['a', 'A', 's', 'S', 'w', 'W', 'd', 'D'];
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
      DrawMessages();
    } else if (action == 'h' || action == 'H') {
      items.AddRange(player.backpack.weapons);
      type = (int)Items.WEAPON;
      ListItems(items, type, "Weapon");
      DrawMessages();
    } else if (action == 'j' || action == 'J') {
      items.AddRange(player.backpack.food);
      type = (int)Items.FOOD;
      ListItems(items, type, "Food");
      DrawMessages();
    } else if (action == 'k' || action == 'K') {
      items.AddRange(player.backpack.potions);
      type = (int)Items.POTION;
      ListItems(items, type, "Potion");
      DrawMessages();
    } else if (action == 'e' || action == 'E') {
      items.AddRange(player.backpack.scrolls);
      type = (int)Items.SCROLL;
      ListItems(items, type, "Scroll");
      DrawMessages();
    }
    if (game.isOver) {
      GameEndMessage(killer);
    }
    NCurses.Refresh();
    if (type != -1)
      ItemUsedMessage(items, game, type);
  }

  public void ItemUsedMessage(List<Item> items, Game game, int type) {
    if (items.Count == 0 && type != (int)Items.WEAPON)
      return;
    if (items.Count == 0 && type == (int)Items.WEAPON && !player.currWeapon.equipped)
      return;
    int choice = SelectItem(items, type);
    NCurses.Erase();
    DrawScene();
    if (choice != CursesKey.ESC) {
      string msg;
      if (choice == '0') {
        msg = string.Format("You put {0} away.", player.currWeapon.name);
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
        if (ok)
          msg = string.Format("You {3} {0} (+{1} {2}).", name, i.value, i.subtype, action);
        else
          msg = "You can't change weapons here.";
      }
      NCurses.AttributeSet(NCurses.ColorPair(5) | CursesAttribute.NORMAL);
      NCurses.MoveAddString(Level.ROWS + MSG_START, X_BORDER + 1, msg);
      NCurses.Refresh();
    }
  }

  public void DrawMessages() {
    NCurses.AttributeSet(NCurses.ColorPair(4) | CursesAttribute.NORMAL);
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
    string effect = "None";
    if (player.effect != "")
      effect = string.Format("{0} ({1})", player.effect, player.effCount);
    NCurses.MoveAddString(
        Level.ROWS + 1, X_BORDER + 1,
        string.Format("LVL: {0}(21), HP: {1}({2}), STR: {3}, AGL: {4}, EFFECTS: {5}", player.lvl,
                      player.hp, player.hp_max, player.str, player.agl, effect));
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
    messages.Clear();
    messages.Enqueue(string.Format("Treasure: {0} Coins", player.GetTreasure()));
    messages.Enqueue(string.Format("Potions: {0} (Press K to Use)", player.backpack.potions.Count));
    messages.Enqueue(string.Format("Scrolls: {0} (Press E to Use)", player.backpack.scrolls.Count));
    messages.Enqueue(string.Format("Food: {0} (Press J to Use)", player.backpack.food.Count));
    messages.Enqueue(string.Format("Weapons: {0} (Press H to Use)", player.backpack.weapons.Count));
  }

  public void ListItems(List<Item> items, int type, string stype) {
    messages.Clear();
    if (items.Count == 0 && type != (int)Items.WEAPON) {
      messages.Enqueue("There is nothing here.");
      return;
    }
    if (items.Count == 0 && type == (int)Items.WEAPON && !player.currWeapon.equipped) {
      messages.Enqueue("There is nothing here.");
      return;
    }
    int begin =
        (items.Count < 9 && type == (int)Items.WEAPON && player.currWeapon.equipped) ? 0 : 1;
    messages.Enqueue(string.Format("Choose {0} ({1}-9) | ESC to return:", stype, begin));
    if (items.Count < 9 && type == (int)Items.WEAPON && player.currWeapon.equipped) {
      messages.Enqueue("0. Unequip current weapon");
      begin++;
    }
    foreach (var i in items) {
      string name;
      if (i is Weapon || i is Food)
        name = i.name;
      else
        name = string.Format("{0} of {1}", i.type, i.subtype);
      messages.Enqueue(string.Format("{0}. {1} ({2} +{3})", begin, name, i.subtype, i.value));
      begin++;
    }
  }

  public int SelectItem(List<Item> items, int type) {
    int c = 0;
    int min = (items.Count < 9 && type == (int)Items.WEAPON && player.currWeapon.equipped) ? 0 : 1;
    while (c != CursesKey.ESC) {
      c = NCurses.GetChar();
      if (c - '0' >= min && c - '0' <= items.Count)
        break;
    }
    return c;
  }

  public void GameEndMessage(string killer) {
    NCurses.AttributeSet(NCurses.ColorPair(3) | CursesAttribute.NORMAL);
    string msg = string.Format("You were defeated by {0}!", killer);
    NCurses.MoveAddString(Level.ROWS + MSG_START + messages.Count, X_BORDER + 1, msg);
  }
}