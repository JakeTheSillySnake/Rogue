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
      // handle damage to enemy
      List<int> attackResult = game.player.Move(c, game.lvl);
      bool dead = game.lvl.ProcessDamage(attackResult);

      string killer = game.UpdateGame();

      NCurses.Erase();
      game.DrawField();
      game.DrawPlayer();
      game.DrawEnemies();
      game.DrawMessages(attackResult, dead);

      if (game.isOver) {
        game.GameEndMessage(killer);
      }
      NCurses.Refresh();
      c = NCurses.GetChar();
    }
    NCurses.EndWin();
  }
}
