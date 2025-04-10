namespace rogue1980.domain
{
    public interface ILevelFactory
    {
        public int[,] createLevelMap(int sizeY, int sizeX, int difficulty);
    }
}