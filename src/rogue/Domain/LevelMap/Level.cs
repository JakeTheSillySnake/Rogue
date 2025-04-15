namespace rogue.Domain.LevelMap;

public class Level {
  public const int ROWS = 22, COLS = 80;
  public int[,] field = new int[ROWS, COLS];
  public static int enemyCode = 100, itemCode = 1000;

  public List<Enemy> enemies = [];
  public List<Item> items = [];
  public List<Door> doors = [];
  public List<Room> rooms = [];
  public List<(Route, int)> corridors = [];

  public Level(int difficulty) {
    var factory = new LevelFactory();
    var result = factory.createLevelMap(ROWS, COLS, difficulty);
    field = result.Item1;
    rooms = result.Item2;
    corridors = result.Item3;
    TransformRawMap();
  }

  public void TransformRawMap() {
    Random rnd = new();
    for (int y = 0; y < ROWS; y++) {
      for (int x = 0; x < COLS; x++) {
        if (field[y, x] == (int)MapCellStates.ENEMY) {
          int enemy = rnd.Next(Enum.GetNames(typeof(Enemies)).Length);
          SpawnEnemy(enemy, x, y);
        }
        if (field[y, x] == (int)MapCellStates.ITEM) {
          int item = rnd.Next(Enum.GetNames(typeof(Items)).Length - 2);
          SpawnItem(item, x, y);
        }
        if (field[y, x] == (int)MapCellStates.KEY_BLUE)
          SpawnKey((int)Colors.BLUE, x, y);
        if (field[y, x] == (int)MapCellStates.KEY_RED)
          SpawnKey((int)Colors.RED, x, y);
        if (field[y, x] == (int)MapCellStates.KEY_GREEN)
          SpawnKey((int)Colors.GREEN, x, y);
        if (field[y, x] == (int)MapCellStates.DOOR_BLUE)
          SpawnDoor((int)Colors.BLUE, x, y);
        if (field[y, x] == (int)MapCellStates.DOOR_RED)
          SpawnDoor((int)Colors.RED, x, y);
        if (field[y, x] == (int)MapCellStates.DOOR_GREEN)
          SpawnDoor((int)Colors.GREEN, x, y);
      }
    }
  }

  public List<int> GetStartPos() {
    List<int> res = [0, 0];
    for (int y = 0; y < ROWS; y++) {
      for (int x = 0; x < COLS; x++) {
        if (field[y, x] == (int)MapCellStates.ENTER) {
          res[0] = y;
          res[1] = x;
          return res;
        }
      }
    }
    return res;
  }

  public List<int> GetEndPos() {
    List<int> res = [0, 0];
    for (int y = 0; y < ROWS; y++) {
      for (int x = 0; x < COLS; x++) {
        if (field[y, x] == (int)MapCellStates.EXIT) {
          res[0] = y;
          res[1] = x;
          return res;
        }
      }
    }
    return res;
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
      items.Add(new Treasure());;
    int idx = items.Count - 1;
    items[idx].Spawn(x, y);
  }

  public void SpawnKey(int color, int x, int y) {
    items.Add(new Key(color));
    int idx = items.Count - 1;
    items[idx].Spawn(x, y);
  }

  public void SpawnDoor(int color, int x, int y) {
    doors.Add(new Door(y, x, (int)DoorLockState.LOCKED));
    int idx = doors.Count - 1;
    doors[idx].SetColor(color);
  }

  public bool DropWeapon(Player p) {
    var w = new Weapon { name = p.currWeapon.name, value = p.currWeapon.value };
    int x = p.x, y = p.y;
    if (p.CheckLeft(this, 1))
      x--;
    else if (p.CheckRight(this, 1))
      x++;
    else if (p.CheckDown(this, 1))
      y++;
    else if (p.CheckUp(this, 1))
      y--;
    if (x == p.x && y == p.y)
      return false;
    items.Add(w);
    w.Spawn(x, y);
    w.floorState = field[y, x];
    field[y, x] = itemCode + items.Count - 1;
    return true;
  }

  public void PickUpWeapon(Weapon w) {
    field[w.y, w.x] = w.floorState;
  }

  public void UpdateField() {
    for (int i = 0; i < ROWS; i++) {
      for (int j = 0; j < COLS; j++) {
        if (field[i, j] >= enemyCode)
          field[i, j] = (int)MapCellStates.EMPTY;
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
    foreach (var d in doors) {
      if (d.lockState == (int)DoorLockState.OPEN)
        field[d.posY, d.posX] = (int)MapCellStates.DOOR;
    }
  }

  public bool ProcessDamage(List<int> res, int difficulty) {
    if (res[1] == 0)
      return false;
    int pos = res[0] - enemyCode;
    bool dead = enemies[pos].ProcessDamage(res[1]);
    int treasure = enemies[pos].GenTreasure() + difficulty;
    if (dead) {
      SpawnItem((int)Items.TREASURE, enemies[pos].x, enemies[pos].y);
      items[^1].value = treasure;
    }
    return dead;
  }
}