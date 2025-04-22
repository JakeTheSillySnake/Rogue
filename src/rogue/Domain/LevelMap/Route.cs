namespace rogue.Domain.LevelMap {
  public class Route {
    public List<(int posY, int posX)> tiles { get; private set; }
    public bool visited { get; set; }
    public Route(int posYA, int posYB, int posXA, int posXB) {
      visited = false;
      tiles = [(posYA, posXA)];
      List<(double distance, int posY, int posX)> bestTile = [];

      while (GetDistanceCoords(tiles.Last().posY, posYB, tiles.Last().posX, posXB) >= 1) {
        bestTile.Add((GetDistanceCoords(tiles.Last().posY - 1, posYB, tiles.Last().posX, posXB),
                      tiles.Last().posY - 1, tiles.Last().posX));
        bestTile.Add((GetDistanceCoords(tiles.Last().posY + 1, posYB, tiles.Last().posX, posXB),
                      tiles.Last().posY + 1, tiles.Last().posX));
        bestTile.Add((GetDistanceCoords(tiles.Last().posY, posYB, tiles.Last().posX - 1, posXB),
                      tiles.Last().posY, tiles.Last().posX - 1));
        bestTile.Add((GetDistanceCoords(tiles.Last().posY, posYB, tiles.Last().posX + 1, posXB),
                      tiles.Last().posY, tiles.Last().posX + 1));

        bestTile.Sort();

        tiles.Add((bestTile.First().posY, bestTile.First().posX));
      }
    }

    public double GetDistanceCoords(int posYA, int posYB, int posXA, int posXB) {
      return Math.Sqrt(Math.Pow(posYB - posYA, 2) + Math.Pow(posXB - posXA, 2));
    }

    public bool ContainsTarget(int x, int y) {
      foreach (var (posY, posX) in tiles) {
        if (posX == x && posY == y)
          return true;
      }
      return false;
    }
  }
}
