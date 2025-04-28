namespace rogue.Domain.LevelMap {
  public class Tile {
    public int PosY { get; set; }
    public int PosX { get; set; }

    public Tile(int posY, int posX) {
      PosY = posY;
      PosX = posX;
    }
  }
}
