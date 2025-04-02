using Mindmagma.Curses;

using View.Game;

class Program {
  static void Main() {
    var Screen = NCurses.InitScreen();
    NCurses.NoDelay(Screen, false);
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    int c = 0;
    Game game = new Game();

    while (c != 'Q' && c != 'q' && c != CursesKey.ESC && !game.isOver) {
      NCurses.Erase();
      if (c == 'i' || c == 'I') {
        game.DrawScene();
        game.ListInventory();
        NCurses.Refresh();
        c = NCurses.GetChar();
        continue;
      }
      var killer = game.UpdateGame(c);
      game.DrawScene();
      game.DrawMessages();
      if (game.isOver) {
        game.GameEndMessage(killer);
      }
      NCurses.Refresh();
      c = NCurses.GetChar();
    }
    NCurses.EndWin();
  }
}
