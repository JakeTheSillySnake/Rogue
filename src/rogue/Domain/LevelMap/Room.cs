namespace rogue.Domain.LevelMap {
  public class Room {
    public int startPosX { get; private set; }
    public int startPosY { get; private set; }
    public int endPosX { get; private set; }
    public int endPosY { get; private set; }
    public int centerPosX { get; private set; }
    public int centerPosY { get; private set; }
    public bool visited { get; set; } = false;

    public Room() : this(new Random(), 0, 0, 9, 9) {}
    public Room(Random random, int startLimitPosY, int startLimitPosX, int endLimitPosY,
                int endLimitPosX) {
      centerPosY = (startLimitPosY + endLimitPosY) / 2;
      centerPosX = (startLimitPosX + endLimitPosX) / 2;

      startPosY = random.Next(startLimitPosY + 1, centerPosY - 1);
      startPosX = random.Next(startLimitPosX + 1, centerPosX - 1);
      endPosY = random.Next(centerPosY + 1, endLimitPosY - 1);
      endPosX = random.Next(centerPosX + 1, endLimitPosX - 1);

      centerPosY = (startPosY + endPosY) / 2;
      centerPosX = (startPosX + endPosX) / 2;
    }
    public bool ContainsTarget(int x, int y) {
      if (x > startPosX && x < endPosX && y > startPosY && y < endPosY)
        return true;
      else
        return false;
    }
  }
}
