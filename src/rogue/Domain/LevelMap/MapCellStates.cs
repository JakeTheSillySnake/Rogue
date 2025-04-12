namespace rogue.Domain.LevelMap {
  enum MapCellStates {
    EMPTY = 0,
    CORRIDOR,
    ENTER,
    EXIT,
    WALL,
    DOOR,
    BUSY,
    ENEMY,
    ITEM,
    KEY_DOOR = 9,
    KEY_DOOR_RED = 10,
    KEY_DOOR_GREEN = 11,
    KEY_DOOR_BLUE = 12,
  }

enum Colors {
  BLUE = 1,
  WHITE,
  RED,
  YELLOW,
  GREEN,
  MAGENTA
}
}
