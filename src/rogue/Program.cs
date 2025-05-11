using Mindmagma.Curses;
using rogue.View;

class Program {
  static void Main() {
    bool error = false;
    NCurses.InitScreen();
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    Scene scene = new();
    try {
      scene.Start();
    } catch {
      error = true;
    }
    NCurses.EndWin();
    if (error)
      Console.WriteLine("Couldn't display graphics: terminal size too small.");
  }
}
