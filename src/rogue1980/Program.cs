using Domain.Level;
using Mindmagma.Curses;
using rogue1980.domain;
using System.Diagnostics.Tracing;
using View.Game;

class Program {
  static void Main() {

        int[,] map = LevelFactory.createLevelMap(5 * 9, 70);

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] != 0)
                {
                    Console.Write(map[y, x]);
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.Write('\n');
        }
        //var Screen = NCurses.InitScreen();
        //NCurses.NoDelay(Screen, false);
        //NCurses.NoEcho();
        //NCurses.SetCursor(0);
        //int c = 0;
        //Game game = new Game();

        //while (c != 'Q' && c != 'q' && c != CursesKey.ESC && !game.isOver) {
        //  // handle damage to enemy
        //  List<int> attackResult = game.player.Move(c, game.lvl);
        //  bool dead = game.lvl.ProcessDamage(attackResult);

        //  string killer = game.UpdateGame();

        //  NCurses.Erase();
        //  game.DrawField();
        //  game.DrawPlayer();
        //  game.DrawEnemies();
        //  game.DrawMessages(attackResult, dead);

        //  if (game.isOver) {
        //    game.GameEndMessage(killer);
        //  }
        //  NCurses.Refresh();
        //  c = NCurses.GetChar();
        //}
        //NCurses.EndWin();

    }
}

/*
int[,] map = LevelFactory.createLevelMap(5 * 9, 70);

for (int y = 0; y < map.GetLength(0); y++)
{
    for (int x = 0; x < map.GetLength(1); x++)
    {
        if (map[y, x] != 0)
        {
            Console.Write(map[y, x]);
        }
        else
        {
            Console.Write('map[y, x]');
        }
    }
    Console.Write('\n');
}
Консольный рендер (не стирать, им уровни чекаю), ибо устал с виртуалки сидеть*/