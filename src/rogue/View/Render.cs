namespace rogue.View;

using rogue.Domain.LevelMap;
using rogue.Domain;

public class Render {
  public int[,] fieldMask = new int[Level.ROWS, Level.COLS];
  public int intensity = 4;
  public Render() {}

  public int[,] CreateLevelMask(Level lvl, Player p) {
    for (int y = 0; y < Level.ROWS; y++) {
      for (int x = 0; x < Level.COLS; x++) {
        fieldMask[y, x] = lvl.field[y, x];
      }
    }
    RenderRooms(lvl, p);
    RenderCorridors(lvl, p);
    return fieldMask;
  }

  public void RenderRooms(Level lvl, Player p) {
    foreach (var room in lvl.rooms) {
      if (!room.ContainsTarget(p.x, p.y)) {
        for (int x = room.startPosX + 1; x < room.endPosX; x++) {
          for (int y = room.startPosY + 1; y < room.endPosY; y++) {
            if (p.DistanceToTarget(x, y) >= intensity)
              fieldMask[y, x] = (int)MapCellStates.BUSY;
          }
        }
      } else {
        room.visited = true;
      }
      if (!room.visited) {
        for (int y = room.startPosY; y <= room.endPosY; y++) {
          if (p.DistanceToTarget(room.startPosX, y) >= intensity)
            fieldMask[y, room.startPosX] = (int)MapCellStates.BUSY;
          if (p.DistanceToTarget(room.endPosX, y) >= intensity)
            fieldMask[y, room.endPosX] = (int)MapCellStates.BUSY;
        }
        for (int x = room.startPosX; x <= room.endPosX; x++) {
          if (p.DistanceToTarget(x, room.startPosY) >= intensity)
            fieldMask[room.startPosY, x] = (int)MapCellStates.BUSY;
          if (p.DistanceToTarget(x, room.endPosY) >= intensity)
            fieldMask[room.endPosY, x] = (int)MapCellStates.BUSY;
        }
      }
    }
  }

  public void RenderCorridors(Level lvl, Player p) {
    foreach (var cor in lvl.corridors) {
      if (cor.Item1.ContainsTarget(p.x, p.y))
        cor.Item1.visited = true;
      if (!cor.Item1.visited) {
        foreach ((int posY, int posX) in cor.Item1.tiles) {
          if (p.DistanceToTarget(posX, posY) >= intensity)
            fieldMask[posY, posX] = (int)MapCellStates.BUSY;
        }
      }
    }
  }
}