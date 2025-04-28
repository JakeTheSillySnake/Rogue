using Mindmagma.Curses;
using rogue.View;

class Program {
  static void Main() {
    NCurses.InitScreen();
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    Scene scene = new();
    scene.Start();
    NCurses.EndWin();
  }
}
