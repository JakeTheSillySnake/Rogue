using System;
using Mindmagma.Curses;

//Console.WriteLine("Hello, World!");

class Game {
    private const int FRAMERATE = 150;
    static void Main() {
        var Screen = NCurses.InitScreen();
        NCurses.NoDelay(Screen, true);
        NCurses.NoEcho();
        int c = '\0';

        while (c != 'Q' && c != 'q') {
            c = NCurses.GetChar();
            NCurses.Nap(FRAMERATE);
            NCurses.StartColor();
            NCurses.InitPair(1, CursesColor.BLUE, CursesColor.BLACK);
            NCurses.AttributeSet(NCurses.ColorPair(1));
            NCurses.MoveAddString(0, 0, "Press 'Q' to quit.");
            NCurses.Refresh();
        }
        NCurses.EndWin();
    }
}
