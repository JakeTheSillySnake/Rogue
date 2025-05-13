namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

public class Ghost : Enemy {
  private int _timer { get; set; } = 5;
  private int _minX { get; set; } = 0;
  private int _maxX { get; set; } = 0;
  private int _minY { get; set; } = 0;
  private int _maxY { get; set; } = 0;

  public Ghost(int x, int y) {
    Symbol = "g";
    Hp = 4 * valLow;
    Hp_max = 4 * valLow;
    Str = valLow;
    Agl = valHigh;
    Color = (int)Colors.WHITE;
    Hostility = 2;
    InitCoords(x, y);
  }

  public override void Move(Level lvl) {
    if (_minX == 0 && _maxX == 0)
      LoadRooms(lvl.rooms);
    if (_timer == 0) {
      Random rnd = new();
      do {
        PosX = rnd.Next(_minX, _maxX);
        PosY = rnd.Next(_minY, _maxY);
      } while (lvl.field[PosY, PosX] == (int)MapCellStates.EXIT);
      _timer = 6;
      // 1/3 chance to become invisible
      if (rnd.Next(1, 4) == 1 && !Follow)
        Symbol = "";
      else
        Symbol = "g";
    } else
      _timer--;
  }

  public void LoadRooms(List<Room> rooms) {
    foreach (var room in rooms) {
      if (room.ContainsTarget(PosX, PosY)) {
        _minX = room.startPosX + 1;
        _maxX = room.endPosX;
        _minY = room.startPosY + 1;
        _maxY = room.endPosY;
        break;
      }
    }
  }

  public override void ChangeSymbol() {
    Symbol = "g";
  }
}