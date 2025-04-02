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
        game.ListInventory();
        NCurses.Refresh();
        c = NCurses.GetChar();
        continue;
      }

      // handle damage to enemy
      List<int> attackResult = game.player.Move(c, game.lvl);
      var kill = game.lvl.ProcessDamage(attackResult);
      if (kill.Item1) game.player.backpack.AddTreasure(kill.Item2);

      var killer = game.UpdateGame();
      game.DrawField();
      game.DrawPlayer();
      game.DrawEnemies();
      game.DrawMessages(attackResult, kill.Item1, kill.Item2);

      if (game.isOver) {
        game.GameEndMessage(killer);
      }
      NCurses.Refresh();
      c = NCurses.GetChar();
    }
    NCurses.EndWin();
  }
}
