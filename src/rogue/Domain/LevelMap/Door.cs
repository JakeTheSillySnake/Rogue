namespace rogue.Domain.LevelMap {
  public class Door {
    public int posY { get; private set; }
    public int posX { get; private set; }
    public int lockState { get; set; }
    public int color = (int)Colors.WHITE;

    public Door(int posY, int posX, int lockState) {
      this.posY = posY;
      this.posX = posX;
      this.lockState = lockState;
    }
    public void SetColor(int color) {
      this.color = color;
    }
  }
}
