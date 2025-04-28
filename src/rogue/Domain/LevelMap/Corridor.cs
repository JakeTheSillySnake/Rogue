namespace rogue.Domain.LevelMap
{
    public class Corridor
    {
        public Route route { get; set; }
        public int lockCode { get; set; }
        public Corridor(Route route, int lockCode)
        {
            this.route = route;
            this.lockCode = lockCode;
        }
    }
}