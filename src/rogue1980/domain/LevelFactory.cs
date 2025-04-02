namespace rogue1980.domain
{
    public static class LevelFactory
    {
        public static int[,] createLevelMap(int sizeY, int sizeX) {

            int[,] map = new int[sizeY, sizeX];
            Random random = new Random();

            List<Room> rooms = GenerateRooms(map, random);
            List<(Room, Room)> routeMST = GenerateMST(map, random, rooms);
            List<(Door, Door)> doorMST = GenerateDoors(map, random, routeMST);
            List<Route> routes = GenerateCorridors(map, random, doorMST);

            AssembleMap(map, rooms, routes, doorMST);

            return map;
        }

        static List<Room> GenerateRooms(int[,] map, Random random)
        {
            List<Room> rooms = new List<Room>();

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    rooms.Add(new Room(
                        random, 
                        map.GetLength(0) / 3 * y,
                        map.GetLength(1) / 3 * x,
                        map.GetLength(0) / 3 * (y + 1),
                        map.GetLength(1) / 3 * (x + 1)
                        ));
                }
            }

            return rooms;
        }

        static List<(Room, Room)> GenerateMST(int[,] map, Random random, List<Room> rooms)
        {
            List<(Room, Room)> roomMST = new List<(Room, Room)>();

            List<Room> visitedRooms = new List<Room>() { rooms[0] };

            List<Room> nonVisitedRooms = new List<Room>(rooms);
            nonVisitedRooms.Remove(rooms[0]);

            while (visitedRooms.Count < 9) {
                (double distance, Room room1, Room room2) bestRoute = new(double.MaxValue, new Room(), new Room());
                foreach (Room oldRoom in visitedRooms) {
                    foreach (Room newRoom in nonVisitedRooms)
                    {
                        if (GetDistanceBetweenRooms(oldRoom, newRoom) < bestRoute.distance)
                        {
                            bestRoute.distance = GetDistanceBetweenRooms(oldRoom, newRoom);
                            bestRoute.room1 = oldRoom;
                            bestRoute.room2 = newRoom;
                        }
                    } 
                }

                roomMST.Add((bestRoute.room1, bestRoute.room2));
                visitedRooms.Add(bestRoute.room2);
                nonVisitedRooms.Remove(bestRoute.room2);
            }

            return roomMST;
        }

        private static List<(Door, Door)> GenerateDoors(int[,] map, Random random, List<(Room, Room)> roomMST)
        {
            List<(Door, Door)> doorMST = new List<(Door, Door)>();

            foreach ((Room roomA, Room roomB) route in roomMST) {
                
                int diffY = route.roomB.centerPosY - route.roomA.centerPosY;
                int diffX = route.roomB.centerPosX - route.roomA.centerPosX;

                if (Math.Abs(diffY) > Math.Abs(diffX)) {
                    if (diffY > 0) {
                        doorMST.Add((
                            new Door(route.roomA.endPosY + 1, route.roomA.centerPosX), 
                            new Door(route.roomB.startPosY - 1, route.roomB.centerPosX)
                        ));
                    }
                    else
                    {
                        doorMST.Add((
                            new Door(route.roomA.startPosY - 1, route.roomA.centerPosX),
                            new Door(route.roomB.endPosY + 1, route.roomB.centerPosX)
                        ));
                    }
                } else {
                    if (diffX > 0)
                    {
                        doorMST.Add((
                            new Door(route.roomA.centerPosY, route.roomA.endPosX + 1),
                            new Door(route.roomB.centerPosY, route.roomB.startPosX - 1)
                        ));
                    }
                    else
                    {
                        doorMST.Add((
                           new Door(route.roomA.centerPosY, route.roomA.startPosX - 1),
                           new Door(route.roomB.centerPosY, route.roomB.endPosX + 1)
                        ));
                    }
                }
            }

            return doorMST;
        }

        private static List<Route> GenerateCorridors(int[,] map, Random random, List<(Door, Door)> doorMST)
        {
            List<Route> routes = new List<Route>();

            foreach (var routePointPair in doorMST)
            {
                routes.Add(
                    new Route(
                        routePointPair.Item1.posY,
                        routePointPair.Item2.posY,
                        routePointPair.Item1.posX,
                        routePointPair.Item2.posX));    
            }

            return routes;
        }

        private static void AssembleMap(int[,] map, List<Room> rooms, List<Route> routes, List<(Door, Door)> doorMST)
        {
            foreach (Room room in rooms)
            {
                for (int y = room.startPosY; y <= room.endPosY; y++)
                {
                    map[y, room.startPosX] = (int)CellStates.WALL;
                    map[y, room.endPosX] = (int)CellStates.WALL;
                }
                for (int x = room.startPosX; x <= room.endPosX; x++)
                {
                    map[room.startPosY, x] = (int)CellStates.WALL;
                    map[room.endPosY, x] = (int)CellStates.WALL;
                }
            }

            foreach (Route route in routes)
            {
                foreach ((int posY, int posX) in route.tiles)
                {
                    map[posY, posX] = (int)CellStates.CORRIDOR;
                }
            }

            foreach ((Door, Door) doorPair in doorMST)
            {
                for (int y = -1; y < 2; y++) {
                    for (int x = -1; x < 2; x++)
                    {
                        if (map[y + doorPair.Item1.posY, x + doorPair.Item1.posX] == (int)CellStates.WALL && Math.Abs(y + x) % 2 == 1)
                        {
                            map[y + doorPair.Item1.posY, x + doorPair.Item1.posX] = (int)CellStates.DOOR;
                        }
                        if (map[y + doorPair.Item2.posY, x + doorPair.Item2.posX] == (int)CellStates.WALL && Math.Abs(y + x) % 2 == 1)
                        {
                            map[y + doorPair.Item2.posY, x + doorPair.Item2.posX] = (int)CellStates.DOOR;
                        }
                    }
                }
            }

        }

        public static double GetDistanceBetweenRooms(Room roomA, Room roomB)
        {
            return Math.Sqrt(Math.Pow(roomB.centerPosX - roomA.centerPosX, 2) + Math.Pow(roomB.centerPosY - roomA.centerPosY, 2));
        }

        public static double GetDistanceBetweenCoords(int posYA, int posYB, int posXA, int posXB)
        {
            return Math.Sqrt(Math.Pow(posYB - posYA, 2) + Math.Pow(posXB - posXA, 2));
        }
    }
}
