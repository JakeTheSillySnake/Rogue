using Mindmagma.Curses;
using rogue.Data;
using rogue.Domain;
using rogue.Domain.LevelMap;
using rogue.View;

class Program {
  static void Main() {
    Level l = new Level(5);
    Player player = new(0, 0);
    Statistics statistics = new Statistics();
    SessionDataSaver.LoadData(l, player, statistics);

    //SessionData a = SessionDataSaver.GetSessionData();
    /*NCurses.InitScreen();
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    Scene scene = new();
    scene.Start();
    NCurses.EndWin();*/
  }
}
