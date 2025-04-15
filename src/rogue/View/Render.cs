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
            if (!InLineOfSight(p.x, p.y, x, y))
              fieldMask[y, x] = (int)MapCellStates.BUSY;
          }
        }
      } else {
        room.visited = true;
      }
      if (!room.visited) {
        for (int y = room.startPosY; y <= room.endPosY; y++) {
          if (!InLineOfSight(p.x, p.y, room.startPosX, y))
            fieldMask[y, room.startPosX] = (int)MapCellStates.BUSY;
          if (!InLineOfSight(p.x, p.y, room.endPosX, y))
            fieldMask[y, room.endPosX] = (int)MapCellStates.BUSY;
        }
        for (int x = room.startPosX; x <= room.endPosX; x++) {
          if (!InLineOfSight(p.x, p.y, x, room.startPosY))
            fieldMask[room.startPosY, x] = (int)MapCellStates.BUSY;
          if (!InLineOfSight(p.x, p.y, x, room.endPosY))
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
          if (!InLineOfSight(p.x, p.y, posX, posY))
            fieldMask[posY, posX] = (int)MapCellStates.BUSY;
        }
      }
    }
  }

  public bool InLineOfSight(int sourceX, int sourceY, int targetX, int targetY) {
    int x = sourceX, y = sourceY;
    int wall = (int)MapCellStates.WALL, busy = (int)MapCellStates.BUSY;
    int deltaX = Math.Abs(sourceX - targetX), deltaY = Math.Abs(sourceY - targetY);
    int dist = (int)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
    int decisionParam = 2 * deltaY - deltaX;
    if (sourceX == targetX)
      while (y != targetY && fieldMask[y, x] != wall && fieldMask[y, x] != busy)
        y = y > targetY ? y - 1 : y + 1;
    if (sourceY == targetY)
      while (x != targetX && fieldMask[y, x] != wall && fieldMask[y, x] != busy)
        x = x > targetX ? x - 1 : x + 1;
    // bresenham’s algorithm
    while (x != targetX && y != targetY && fieldMask[y, x] != busy && fieldMask[y, x] != wall) {
      if (decisionParam < 0) {
        decisionParam += 2 * deltaY;
      } else {
        y = y > targetY ? y - 1 : y + 1;
        decisionParam += 2 * (deltaY - deltaX);
      }
      x = x > targetX ? x - 1 : x + 1;
    }
    if (fieldMask[y, x] == wall || fieldMask[y, x] == busy || dist >= intensity)
      return false;
    return true;
  }
}