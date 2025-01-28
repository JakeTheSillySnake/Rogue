using Mindmagma.Curses;
using Domain;

class Game {
  private const int FRAMERATE = 150;
  static void Main() {
    int row = 0, col = 0;
    var Screen = NCurses.InitScreen();
    NCurses.NoDelay(Screen, true);
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    int c = 0;

    NCurses.StartColor();
    NCurses.InitPair(1, CursesColor.BLUE, CursesColor.BLACK);
    NCurses.InitPair(2, CursesColor.WHITE, CursesColor.BLACK);
    NCurses.InitPair(3, CursesColor.RED, CursesColor.BLACK);
    NCurses.InitPair(4, CursesColor.YELLOW, CursesColor.BLACK);
    NCurses.InitPair(5, CursesColor.GREEN, CursesColor.BLACK);

    var player = new Player();
    var z = new Zombie();
    var v = new Vampire();
    var g = new Ghost();
    var o = new Ogre();
    var s = new Snake();

    while (c != 'Q' && c != 'q') {
      c = NCurses.GetChar();
      if ((c == 'a' || c == 'A') && player.x > 6)
        player.x--;
      if ((c == 'd' || c == 'D') && player.x < 72)
        player.x++;
      if ((c == 'w' || c == 'W') && player.y > 1)
        player.y--;
      if ((c == 's' || c == 'S') && player.y < 19)
        player.y++;

      NCurses.Nap(FRAMERATE);
      NCurses.Erase();

      // field
      NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
      NCurses.MoveAddString(
          0, 5, " ____________________________________________________________________ ");
      NCurses.MoveAddString(
          20, 5, "|____________________________________________________________________|");
      for (int i = 1; i < 20; i++) {
        NCurses.MoveAddString(
            i, 5, "|                                                                    |");
      }

      NCurses.AttributeSet(NCurses.ColorPair(1) | CursesAttribute.BOLD);
      NCurses.MoveAddString(player.y, player.x, player.symbol);

      // enemies
      NCurses.AttributeSet(NCurses.ColorPair(2));
      NCurses.MoveAddString(s.y, s.x, s.symbol);
      NCurses.MoveAddString(g.y, g.x, g.symbol);
      NCurses.AttributeSet(NCurses.ColorPair(3));
      NCurses.MoveAddString(v.y, v.x, v.symbol);
      NCurses.AttributeSet(NCurses.ColorPair(4));
      NCurses.MoveAddString(o.y, o.x, o.symbol);
      NCurses.AttributeSet(NCurses.ColorPair(5));
      NCurses.MoveAddString(z.y, z.x, z.symbol);

      NCurses.AttributeSet(NCurses.ColorPair(2) | CursesAttribute.NORMAL);
      NCurses.MoveAddString(
          21, 6,
          String.Format("Level: {0}(21), Health: {1}({2}), Strength: {3}, Agility: {4}", player.lvl, player.hp,
                        player.hp_max, player.str, player.agl));
      NCurses.Refresh();
    }
    NCurses.EndWin();
  }
}
