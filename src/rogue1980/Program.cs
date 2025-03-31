using Mindmagma.Curses;

using Domain.Player;
using Domain.Enemies;
using Domain.Level;

class Game {
  public const int FRAMERATE = 150, X_BORDER = 5, Y_BORDER = 1;
  static void Main() {
    var Screen = NCurses.InitScreen();
    NCurses.NoDelay(Screen, true);
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    int c = 0;

    NCurses.StartColor();
    NCurses.InitPair((int)Colors.BLUE, CursesColor.BLUE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.WHITE, CursesColor.WHITE, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.RED, CursesColor.RED, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.YELLOW, CursesColor.YELLOW, CursesColor.BLACK);
    NCurses.InitPair((int)Colors.GREEN, CursesColor.GREEN, CursesColor.BLACK);

    var player = new Player();
    var lvl = new Level();

    // enemy test
    var e = new Snake();

    while (c != 'Q' && c != 'q' && c != CursesKey.ESC) {
      c = NCurses.GetChar();
      player.Move(c, lvl);
      e.Act(lvl, player);
      NCurses.Nap(FRAMERATE);
      NCurses.Erase();

      // field
      NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
      NCurses.MoveAddString(
          0, X_BORDER, " ____________________________________________________________________ ");
      NCurses.MoveAddString(
          Level.ROWS, X_BORDER,
          "|____________________________________________________________________|");
      for (int i = 1; i < Level.ROWS; i++) {
        NCurses.MoveAddString(
            i, X_BORDER, "|                                                                    |");
      }

      // draw player
      NCurses.AttributeSet(NCurses.ColorPair(player.color) | CursesAttribute.BOLD);
      NCurses.MoveAddString(player.y + Y_BORDER, player.x + X_BORDER, player.symbol);

      // draw enemy
      NCurses.AttributeSet(NCurses.ColorPair(e.color));
      NCurses.MoveAddString(e.y + Y_BORDER, e.x + X_BORDER, e.symbol);

      NCurses.AttributeSet(NCurses.ColorPair((int)Colors.WHITE) | CursesAttribute.NORMAL);

      // rooms
      for (int i = 0; i < Level.ROWS; i++) {
        for (int j = 0; j < Level.COLS; j++) {
          if (lvl.field[i, j] == 1) {
            NCurses.MoveAddString(i + Y_BORDER, j + X_BORDER, ".");
          }
        }
      }

      // status bar
      NCurses.MoveAddString(
          Level.ROWS + 1, X_BORDER + 1,
          String.Format("Level: {0}(21), Health: {1}({2}), Strength: {3}, Agility: {4}", player.lvl,
                        player.hp, player.hp_max, player.str, player.agl));
      NCurses.Refresh();
    }
    NCurses.EndWin();
  }
}
