using System.Text;
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
      scene.ProcessKeys(c, game);
      c = NCurses.GetChar();
    }
    NCurses.EndWin();
  }
}
