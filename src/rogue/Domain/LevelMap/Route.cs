namespace rogue.Domain.LevelMap {
  public class Route {
    public List<Tile> tiles { get; private set; }
    public bool visited = false;
    public Route(int posYA, int posYB, int posXA, int posXB) {
      tiles = [new Tile(posYA, posXA)];
      List<(double distance, int PosY, int PosX)> bestTile = [];

      while (GetDistanceCoords(tiles.Last().PosY, posYB, tiles.Last().PosX, posXB) >= 1) {
        bestTile.Add((GetDistanceCoords(tiles.Last().PosY - 1, posYB, tiles.Last().PosX, posXB),
                      tiles.Last().PosY - 1, tiles.Last().PosX));
        bestTile.Add((GetDistanceCoords(tiles.Last().PosY + 1, posYB, tiles.Last().PosX, posXB),
                      tiles.Last().PosY + 1, tiles.Last().PosX));
        bestTile.Add((GetDistanceCoords(tiles.Last().PosY, posYB, tiles.Last().PosX - 1, posXB),
                      tiles.Last().PosY, tiles.Last().PosX - 1));
        bestTile.Add((GetDistanceCoords(tiles.Last().PosY, posYB, tiles.Last().PosX + 1, posXB),
                      tiles.Last().PosY, tiles.Last().PosX + 1));

        bestTile.Sort();

        tiles.Add(new Tile(bestTile.First().PosY, bestTile.First().PosX));
      }
    }

    public double GetDistanceCoords(int posYA, int posYB, int posXA, int posXB) {
      return Math.Sqrt(Math.Pow(posYB - posYA, 2) + Math.Pow(posXB - posXA, 2));
    }

    public bool ContainsTarget(int x, int y) {
      foreach (Tile tile in tiles) {
        if (tile.PosX == x && tile.PosY == y)
          return true;
      }
      return false;
    }
  }
}
