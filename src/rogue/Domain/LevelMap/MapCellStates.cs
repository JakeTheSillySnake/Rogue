namespace rogue.Domain.LevelMap {
  enum MapCellStates {
    EMPTY = 0,
    CORRIDOR,
    ENTER,
    EXIT,
    WALL,
    DOOR,
    DOOR_RED,
    DOOR_GREEN,
    DOOR_BLUE,
    BUSY,
    ENEMY,
    ITEM,
    KEY,
    KEY_RED,
    KEY_GREEN,
    KEY_BLUE
  }

  enum Colors { BLUE = 1, WHITE, RED, YELLOW, GREEN, MAGENTA }

  enum DoorLockState { OPEN, LOCKED }
}
