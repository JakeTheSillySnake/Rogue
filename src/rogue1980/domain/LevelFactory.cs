using System.Collections;
using System.Collections.Generic;

namespace rogue1980.domain
{
    public class LevelFactory : ILevelFactory
    {
        public int[,] createLevelMap(int sizeY, int sizeX) {

            int[,] map = new int[sizeY, sizeX];
            Random random = new Random();

            List<Room> rooms = GenerateRooms(map, random);
            List<(Room, Room, int)> roomMST = GenerateMST(map, random, rooms);
            List<int> keyPositions = GenerateDOOM(map, random, rooms, roomMST);
            List<(Door, Door, int)> doorMST = GenerateDoors(map, random, roomMST);
            List<(Route, int)> routes = GenerateCorridors(map, random, doorMST);

            AssembleMap(map, rooms, routes, doorMST, keyPositions);

            return map;
        }

        List<Room> GenerateRooms(int[,] map, Random random)
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

        List<(Room, Room, int)> GenerateMST(int[,] map, Random random, List<Room> rooms)
        {
            List<(Room, Room, int)> roomMST = new List<(Room, Room, int)>();

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

                roomMST.Add((bestRoute.room1, bestRoute.room2, (int)DoorLockState.OPEN));
                visitedRooms.Add(bestRoute.room2);
                nonVisitedRooms.Remove(bestRoute.room2);
            }

            return roomMST;
        }

        private List<(Door, Door, int)> GenerateDoors(int[,] map, Random random, List<(Room, Room, int)> roomMST)
        {
            List<(Door, Door, int)> doorList = new List<(Door, Door, int)>();

            foreach ((Room roomA, Room roomB, int door) route in roomMST) {
                
                int diffY = route.roomB.centerPosY - route.roomA.centerPosY;
                int diffX = route.roomB.centerPosX - route.roomA.centerPosX;

                if (Math.Abs(diffY) > Math.Abs(diffX)) {
                    if (diffY > 0) {
                        doorList.Add((
                            new Door(route.roomA.endPosY + 1, route.roomA.centerPosX, (int)DoorLockState.OPEN),
                            new Door(route.roomB.startPosY - 1, route.roomB.centerPosX, (int)DoorLockState.OPEN),
                            route.door
                            ));
                    }
                    else
                    {
                        doorList.Add((
                            new Door(route.roomA.startPosY - 1, route.roomA.centerPosX, (int)DoorLockState.OPEN),
                            new Door(route.roomB.endPosY + 1, route.roomB.centerPosX, (int)DoorLockState.OPEN),
                            route.door
                        ));
                    }
                } else {
                    if (diffX > 0)
                    {
                        doorList.Add((
                            new Door(route.roomA.centerPosY, route.roomA.endPosX + 1, (int)DoorLockState.OPEN),
                            new Door(route.roomB.centerPosY, route.roomB.startPosX - 1, (int)DoorLockState.OPEN),
                            route.door
                        ));
                    }
                    else
                    {
                        doorList.Add((
                           new Door(route.roomA.centerPosY, route.roomA.startPosX - 1, (int)DoorLockState.OPEN),
                           new Door(route.roomB.centerPosY, route.roomB.endPosX + 1, (int)DoorLockState.OPEN),
                            route.door
                        ));
                    }
                }
            }

            return doorList;
        }

        private List<(Route, int)> GenerateCorridors(int[,] map, Random random, List<(Door, Door, int)> doorMST)
        {
            List<(Route, int)> routes = new List<(Route, int)>();

            foreach (var routePointPair in doorMST)
            {
                routes.Add((
                    new Route(
                        routePointPair.Item1.posY,
                        routePointPair.Item2.posY,
                        routePointPair.Item1.posX,
                        routePointPair.Item2.posX),
                        routePointPair.Item3));
                
            }

            return routes;
        }

        private List<int> GenerateDOOM(int[,] map, Random random, List<Room> rooms, List<(Room, Room, int)> roomMST)
        {
            List<int> keyPositions = [];
            bool correctlyGenerated = false;

            //while (!correctlyGenerated)
            //{
                keyPositions.Clear();
                while (keyPositions.Count < 3)
                {
                    int item = random.Next(0, roomMST.Count - 1);

                    if (!keyPositions.Contains(item))
                    {
                        Room roomA = roomMST[item].Item1;
                        Room roomB = roomMST[item].Item2;
                        roomMST.Remove(roomMST[item]);
                        roomMST.Insert(item, (roomA, roomB, keyPositions.Count + 1));

                        keyPositions.Add(item);
                    }
                }
            //}
            return keyPositions;
        }

        private void AssembleMap(int[,] map, List<Room> rooms, List<(Route, int)> routes, List<(Door, Door, int)> doorMST, List<int> keyPositions)
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

            foreach ((Route, int) route in routes)
            {
                foreach ((int posY, int posX) in route.Item1.tiles)
                {
                    map[posY, posX] = (int)CellStates.CORRIDOR;
                }
                if (route.Item2 != 0)
                {
                    int doorPosY = route.Item1.tiles[0].posY;
                    int doorPosX = route.Item1.tiles[0].posX;
                    map[doorPosY, doorPosX] = 5 + route.Item2;
                }
            }

            foreach ((Door, Door, int) doorPair in doorMST)
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

            int key = 0;
            foreach (int keyPosition in keyPositions)
            {
                key++;
                map[rooms[keyPosition].centerPosY, rooms[keyPosition].centerPosX] = key + 5;
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
