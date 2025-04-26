using Mindmagma.Curses;
using rogue.Data;
using rogue.Domain;
using rogue.Domain.LevelMap;
using rogue.View;
using System.Diagnostics;

class Program {
  static void Main() {
        Statistics statistics = new Statistics();
        List<Statistics> statList = GameOverStatSaver.GetGameOverStatData();
        GameOverStatSaver.AddRunStatistics(statList, statistics);
        GameOverStatSaver.LoadData(statList);

        SessionData sessionData = SessionDataSaver.GetSessionData();
        SessionDataSaver.LoadData(new Level(5), new Player(0,0), statistics);

        /*NCurses.InitScreen();
        NCurses.NoEcho();
        NCurses.SetCursor(0);
        Scene scene = new();
        scene.Start();
        NCurses.EndWin();*/
    }
}
