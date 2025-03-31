namespace Domain.Level;

public class Level {
  public const int ROWS = 20, COLS = 70, EMPTY = 0, WALL = 1;
  public int[,] field = new int[ROWS, COLS];

  public Level() {
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        field[i, j] = EMPTY;
      }
    }
    // basic room
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        if (j == 4 || j == 65) {
          if (i != 0 && i < 18)
            field[i, j] = WALL;
        }
        if (i == 1 || i == 17) {
          if (j > 3 && j < 66)
            field[i, j] = WALL;
        }
      }
    }
  }
  // level generation somewhere here
}