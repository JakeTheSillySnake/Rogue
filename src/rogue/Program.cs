using Mindmagma.Curses;

using rogue.View;

class Program {
  static void Main() {
    NCurses.InitScreen();
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    int c = 0;
    Game game = new();
    Scene scene = new();

    while (c != 'Q' && c != 'q' && c != CursesKey.ESC && !game.isOver) {
      scene.UploadGame(game);
      NCurses.Erase();
      if (c == 'i' || c == 'I') {
        scene.DrawScene();
        scene.ListInventory();
      } else {
        var killer = game.UpdateGame(c);
        scene.DrawScene();
        scene.DrawMessages();
        if (game.isOver) {
          scene.GameEndMessage(killer);
        }
      }
      NCurses.Refresh();
      c = NCurses.GetChar();
    }
    NCurses.EndWin();
  }
}
