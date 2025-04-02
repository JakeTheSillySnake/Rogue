namespace rogue1980.domain
{
    public class Door
    {
        public int posY { get; private set; }
        public int posX { get; private set; }
        
        public Door(int posY, int posX)
        {
            this.posY = posY;
            this.posX = posX;
        }
    }
}
