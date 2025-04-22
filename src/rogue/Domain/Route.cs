using rogue.Domain.LevelMap;

namespace rogue1980.domain
{
    public class Route
    {
        public List<Tile> Tiles { get; set; }
        public Route( int posYA, int posYB, int posXA, int posXB)
        {
            Tiles = [new(posYA, posXA)];
            List<(double distance, int posY, int posX)> bestTile = [];

            while (GetDistanceCoords(Tiles.Last().PosY, posYB, Tiles.Last().PosX, posXB) >= 1)
            {
                bestTile.Add((GetDistanceCoords(Tiles.Last().PosY - 1, posYB, Tiles.Last().PosX, posXB), Tiles.Last().PosY - 1, Tiles.Last().PosX));
                bestTile.Add((GetDistanceCoords(Tiles.Last().PosY + 1, posYB, Tiles.Last().PosX, posXB), Tiles.Last().PosY + 1, Tiles.Last().PosX));
                bestTile.Add((GetDistanceCoords(Tiles.Last().PosY, posYB, Tiles.Last().PosX - 1, posXB), Tiles.Last().PosY, Tiles.Last().PosX - 1));
                bestTile.Add((GetDistanceCoords(Tiles.Last().PosY, posYB, Tiles.Last().PosX + 1, posXB), Tiles.Last().PosY, Tiles.Last().PosX + 1));
            
                bestTile.Sort();

                Tiles.Add(new Tile(bestTile.First().posY, bestTile.First().posX));
            }
        }

        public double GetDistanceCoords(int posYA, int posYB, int posXA, int posXB)
        {
            return Math.Sqrt(Math.Pow(posYB - posYA, 2) + Math.Pow(posXB - posXA, 2));
        }
    }
}
