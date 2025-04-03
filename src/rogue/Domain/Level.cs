namespace rogue.Domain;

enum CellStates { EMPTY = 0, WALL }

public class Level {
  public const int ROWS = 20, COLS = 70;
  public int[,] field = new int[ROWS, COLS];
  public static int enemyCode = 100, itemCode = 1000;

  public List<Enemy> enemies = [];
  public List<Item> items = [];

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
    enemies.Clear();
    items.Clear();
  }

  public void SpawnEnemy(int type, int x, int y) {
    if (type == (int)Enemies.ZOMBIE)
      enemies.Add(new Zombie(x, y));
    else if (type == (int)Enemies.VAMPIRE)
      enemies.Add(new Vampire(x, y));
    else if (type == (int)Enemies.OGRE)
      enemies.Add(new Ogre(x, y));
    else if (type == (int)Enemies.GHOST)
      enemies.Add(new Ghost(x, y));
    else if (type == (int)Enemies.SNAKE)
      enemies.Add(new Snake(x, y));
    else if (type == (int)Enemies.MIMIC)
      enemies.Add(new Mimic(x, y));
    int idx = enemies.Count - 1;
    field[y, x] = enemyCode + idx;
  }

  public void SpawnItem(int type, int x, int y) {
    if (type == (int)Items.WEAPON)
      items.Add(new Weapon());
    else if (type == (int)Items.POTION)
      items.Add(new Potion());
    else if (type == (int)Items.SCROLL)
      items.Add(new Scroll());
    else if (type == (int)Items.FOOD)
      items.Add(new Food());
    else if (type == (int)Items.TREASURE)
      items.Add(new Treasure());
    int idx = items.Count - 1;
    items[idx].Spawn(x, y);
    field[y, x] = itemCode + idx;
  }

  public void UpdateField() {
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        if (field[i, j] != (int)CellStates.WALL)
          field[i, j] = (int)CellStates.EMPTY;
      }
    }
    for (int i = 0; i < enemies.Count; i++) {
      if (!enemies[i].dead)
        field[enemies[i].y, enemies[i].x] = enemyCode + i;
    }
    for (int i = 0; i < items.Count; i++) {
      if (items[i].active)
        field[items[i].y, items[i].x] = itemCode + i;
    }
  }

  public bool ProcessDamage(List<int> res) {
    if (res[1] == 0)
      return false;
    int pos = res[0] - enemyCode;
    bool dead = enemies[pos].ProcessDamage(res[1]);
    int treasure = enemies[pos].GenTreasure();
    if (dead) {
      SpawnItem((int)Items.TREASURE, enemies[pos].x, enemies[pos].y);
      items[^1].value = treasure;
    }
    return dead;
  }
  // level generation somewhere here
}