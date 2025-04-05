using System.Text;
using Mindmagma.Curses;

using rogue.View;
using rogue1980.domain;

class Program
{

    /*
        Генерация врагов и предметов ( задать место для общего спавна )
        Динамическая сложность ( % от макс хп, либо значение хп в целом )

        ///

        JSON
        Сбор статистики
        Лидерборды
        Маска "тумана войны" ( в фнукцию обновления передаём координаты персонажа, вызывае функцию каждый ход / спавн )
    */
    static void Main()
    {
        ILevelFactory a = new LevelFactory();
        int[,] map = a.createLevelMap(40, 70);
        for (int y = 0; y < 40; y++)
        {
            for (int x = 0; x < 70; x++)
            {
                if (map[y, x] != 0)
                {
                    Console.Write($"{map[y, x]}");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.Write("\n");
        }
    }
        /*static void Main() {
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
        }*/
}
