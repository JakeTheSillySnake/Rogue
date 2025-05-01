using Mindmagma.Curses;
using Newtonsoft.Json.Linq;
using rogue.Domain.LevelMap;
using rogue.View;

class Program {
  static void Main() {
    Level level = new Level(5);
        for (int i = 0; i < 22; i++)
        {
            Console.WriteLine(" ");
            for (int j = 0; j < 80; j++)
            {
                if (level.field[i, j] != 9)
                {
                    Console.Write(level.field[i, j].ToString("X"));
                }
                else
                {
                    Console.Write(" ");
                }
            }
        }
    /*NCurses.InitScreen();
    NCurses.NoEcho();
    NCurses.SetCursor(0);
    Scene scene = new();
    scene.Start();
    NCurses.EndWin();*/
  }
}
