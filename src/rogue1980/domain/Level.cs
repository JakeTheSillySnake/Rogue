namespace Domain.Level;

using Domain.Enemies;
using rogue1980.domain;

public class Level {
  public const int ROWS = 20, COLS = 70;
  public int[,] field = LevelFactory.createLevelMap(ROWS, COLS);

  // stores all enemies on level
  public List<Zombie> zombies = [];
  public List<Vampire> vampires = [];
  public List<Ogre> ogres = [];
  public List<Ghost> ghosts = [];
  public List<Snake> snakes = [];
  public List<Mimic> mimics = [];

  public Level() {
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        field[i, j] = (int)CellStates.EMPTY;
      }
    }
    // basic room
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        if (j == 4 || j == 65) {
          if (i != 0 && i < 18)
            field[i, j] = (int)CellStates.WALL;
        }
        if (i == 1 || i == 17) {
          if (j > 3 && j < 66)
            field[i, j] = (int)CellStates.WALL;
        }
      }
    }
  }

  public void ClearLevel() {
    zombies.Clear();
    vampires.Clear();
    ogres.Clear();
    ghosts.Clear();
    snakes.Clear();
    mimics.Clear();
  }

  public void SpawnEnemy(string type, int x, int y) {
    int idx = 0, typeCode = 0;
    switch (type) {
      case "z":
        zombies.Add(new Zombie(x, y));
        typeCode = (int)CellStates.ZOMBIE;
        idx = zombies.Count() - 1;
        break;
      case "v":
        vampires.Add(new Vampire(x, y));
        typeCode = (int)CellStates.VAMPIRE;
        idx = vampires.Count() - 1;
        break;
      case "o":
        ogres.Add(new Ogre(x, y));
        typeCode = (int)CellStates.OGRE;
        idx = ogres.Count() - 1;
        break;
      case "g":
        ghosts.Add(new Ghost(x, y));
        typeCode = (int)CellStates.GHOST;
        idx = ghosts.Count() - 1;
        break;
      case "s":
        snakes.Add(new Snake(x, y));
        typeCode = (int)CellStates.SNAKE;
        idx = snakes.Count() - 1;
        break;
      case "m":
        mimics.Add(new Mimic(x, y));
        typeCode = (int)CellStates.MIMIC;
        idx = mimics.Count() - 1;
        break;
    }
    // saving enemy type & index
    field[y, x] = typeCode * 1000 + idx;
  }

  public void UpdateField() {
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        if (field[i, j] != (int)CellStates.WALL)
          field[i, j] = (int)CellStates.EMPTY;
      }
    }
    for (int i = 0; i < zombies.Count(); i++) {
      if (!zombies[i].dead)
        field[zombies[i].y, zombies[i].x] = (int)CellStates.ZOMBIE * 1000 + i;
    }
    for (int i = 0; i < vampires.Count(); i++) {
      if (!vampires[i].dead)
        field[vampires[i].y, vampires[i].x] = (int)CellStates.VAMPIRE * 1000 + i;
    }
    for (int i = 0; i < ogres.Count(); i++) {
      if (!ogres[i].dead)
        field[ogres[i].y, ogres[i].x] = (int)CellStates.OGRE * 1000 + i;
    }
    for (int i = 0; i < ghosts.Count(); i++) {
      if (!ghosts[i].dead)
        field[ghosts[i].y, ghosts[i].x] = (int)CellStates.GHOST * 1000 + i;
    }
    for (int i = 0; i < snakes.Count(); i++) {
      if (!snakes[i].dead)
        field[snakes[i].y, snakes[i].x] = (int)CellStates.SNAKE * 1000 + i;
    }
    for (int i = 0; i < mimics.Count(); i++) {
      if (!mimics[i].dead)
        field[mimics[i].y, mimics[i].x] = (int)CellStates.MIMIC * 1000 + i;
    }
  }

  public bool ProcessDamage(List<int> res) {
    int typeCode = res[0] / 1000, idx = res[0] - (typeCode * 1000);
    bool dead = false;
    switch (typeCode) {
      case (int)CellStates.ZOMBIE:
        dead = zombies[idx].ProcessDamage(res[1]);
        break;
      case (int)CellStates.VAMPIRE:
        dead = vampires[idx].ProcessDamage(res[1]);
        break;
      case (int)CellStates.OGRE:
        dead = ogres[idx].ProcessDamage(res[1]);
        break;
      case (int)CellStates.GHOST:
        dead = ghosts[idx].ProcessDamage(res[1]);
        break;
      case (int)CellStates.SNAKE:
        dead = snakes[idx].ProcessDamage(res[1]);
        break;
      case (int)CellStates.MIMIC:
        dead = mimics[idx].ProcessDamage(res[1]);
        break;
    }
    return dead;
  }
  // level generation somewhere here
}