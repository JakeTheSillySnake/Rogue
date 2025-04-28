namespace rogue.Domain.LevelMap

{
  public class LevelFactory {
    public (int[,], List<Room>, List<Corridor>)
        CreateLevelMap(int sizeY, int sizeX, int difficulty) {
      int[,] map = new int[sizeY, sizeX];
      for (int y = 0; y < sizeY; y++)
        for (int x = 0; x < sizeX; x++)
          map[y, x] = (int)MapCellStates.BUSY;
      Random random = new();

      List<Room> rooms = GenerateRooms(map, random);
      List<(Room, Room, int)> roomMST = GenerateMST(map, random, rooms);
      List<int> keyPositions = GenerateDOOM(map, random, rooms, roomMST);
      List<(Door, Door, int)> doorMST = GenerateDoors(map, random, roomMST);
      List<Corridor> routes = GenerateCorridors(map, random, doorMST);
      List<(int, int)> enemyPositionList = GenerateEnemySpawns(map, random, rooms, difficulty);
      List<(int, int)> itemPositionList = GenerateItemSpawns(map, random, rooms, difficulty);
      AssembleMap(map, random, rooms, routes, doorMST, keyPositions, enemyPositionList,
                  itemPositionList);

      return (map, rooms, routes);
    }

    List<Room> GenerateRooms(int[,] map, Random random) {
      List<Room> rooms = new List<Room>();

      for (int y = 0; y < 3; y++) {
        for (int x = 0; x < 3; x++) {
          rooms.Add(new Room(random, map.GetLength(0) / 3 * y, map.GetLength(1) / 3 * x,
                             map.GetLength(0) / 3 * (y + 1), map.GetLength(1) / 3 * (x + 1)));
        }
      }

      return rooms;
    }

    List<(Room, Room, int)> GenerateMST(int[,] map, Random random, List<Room> rooms) {
      List<(Room, Room, int)> roomMST = new List<(Room, Room, int)>();

      List<Room> visitedRooms = new List<Room>() { rooms[0] };

      List<Room> nonVisitedRooms = new List<Room>(rooms);
      nonVisitedRooms.Remove(rooms[0]);

      while (visitedRooms.Count < 9) {
        (double distance, Room room1, Room room2) bestRoute =
            new(double.MaxValue, new Room(), new Room());
        foreach (Room oldRoom in visitedRooms) {
          foreach (Room newRoom in nonVisitedRooms) {
            if (GetDistanceBetweenRooms(oldRoom, newRoom) < bestRoute.distance) {
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

    private List<(Door, Door, int)> GenerateDoors(int[,] map, Random random,
                                                  List<(Room, Room, int)> roomMST) {
      List<(Door, Door, int)> doorList = new List<(Door, Door, int)>();

      foreach ((Room roomA, Room roomB, int door)route in roomMST) {
        int diffY = route.roomB.centerPosY - route.roomA.centerPosY;
        int diffX = route.roomB.centerPosX - route.roomA.centerPosX;

        if (Math.Abs(diffY) > Math.Abs(diffX)) {
          if (diffY > 0) {
            doorList.Add(
                (new Door(route.roomA.endPosY + 1, route.roomA.centerPosX, (int)DoorLockState.OPEN),
                 new Door(route.roomB.startPosY - 1, route.roomB.centerPosX,
                          (int)DoorLockState.OPEN),
                 route.door));
          } else {
            doorList.Add(
                (new Door(route.roomA.startPosY - 1, route.roomA.centerPosX,
                          (int)DoorLockState.OPEN),
                 new Door(route.roomB.endPosY + 1, route.roomB.centerPosX, (int)DoorLockState.OPEN),
                 route.door));
          }
        } else {
          if (diffX > 0) {
            doorList.Add(
                (new Door(route.roomA.centerPosY, route.roomA.endPosX + 1, (int)DoorLockState.OPEN),
                 new Door(route.roomB.centerPosY, route.roomB.startPosX - 1,
                          (int)DoorLockState.OPEN),
                 route.door));
          } else {
            doorList.Add(
                (new Door(route.roomA.centerPosY, route.roomA.startPosX - 1,
                          (int)DoorLockState.OPEN),
                 new Door(route.roomB.centerPosY, route.roomB.endPosX + 1, (int)DoorLockState.OPEN),
                 route.door));
          }
        }
      }

      return doorList;
    }

    private List<Corridor> GenerateCorridors(int[,] map, Random random,
                                                 List<(Door, Door, int)> doorMST) {
      List<Corridor> routes = new List<Corridor>();

      foreach (var routePointPair in doorMST) {
        routes.Add(new Corridor(new Route(routePointPair.Item1.posY, routePointPair.Item2.posY,
                              routePointPair.Item1.posX, routePointPair.Item2.posX),
                    routePointPair.Item3));
      }

      return routes;
    }

    private List<int> GenerateDOOM(int[,] map, Random random, List<Room> rooms,
                                   List<(Room, Room, int)> roomMST) {
      List<int> keyPositions = [];
      GenerateDoorsDOOM(random, roomMST);

      List<(int, int, int)> roomIndexMST = new();
      foreach (var route in roomMST) {
        roomIndexMST.Add((rooms.IndexOf(route.Item1), rooms.IndexOf(route.Item2), route.Item3));
      }

      GenerateKeysDOOM(random, keyPositions, roomIndexMST);

      return keyPositions;
    }

    private void GenerateDoorsDOOM(Random random, List<(Room, Room, int)> roomMST) {
      List<int> doorPositions = [];
      while (doorPositions.Count < 3) {
        int item = random.Next(doorPositions.Count > 0 ? doorPositions.Last() : 0,
                               roomMST.Count - 2 + doorPositions.Count);
        if (!doorPositions.Contains(item)) {
          Room roomA = roomMST[item].Item1;
          Room roomB = roomMST[item].Item2;
          roomMST.Remove(roomMST[item]);
          roomMST.Insert(item, (roomA, roomB, doorPositions.Count + 1));
          doorPositions.Add(item);
        }
      }
    }

    private void GenerateKeysDOOM(Random random, List<int> keyPositions,
                                  List<(int, int, int)> roomIndexMST) {
      for (int i = 1; i < 4; i++) {
        List<int> visitedIndexRooms = new List<int>() {};
        List<int> avaivableIndexRooms = new List<int>() { 0 };

        while (avaivableIndexRooms.Count > 0) {
          visitedIndexRooms.Add(avaivableIndexRooms.First());
          foreach (var item in roomIndexMST) {
            if (item.Item1 == avaivableIndexRooms.First() &&
                !visitedIndexRooms.Contains(item.Item2) && item.Item3 < i) {
              avaivableIndexRooms.Add(item.Item2);
            } else if (item.Item2 == avaivableIndexRooms.First() &&
                       !visitedIndexRooms.Contains(item.Item1) && item.Item3 < i) {
              avaivableIndexRooms.Add(item.Item1);
            }
          }
          avaivableIndexRooms.Remove(avaivableIndexRooms.First());
        }

        int roomToChoose =
            visitedIndexRooms[random.Next(visitedIndexRooms.Count / 2, visitedIndexRooms.Count)];
        while (keyPositions.Contains(roomToChoose) && visitedIndexRooms.Count > 1) {
          visitedIndexRooms.Remove(roomToChoose);
          roomToChoose =
              visitedIndexRooms[random.Next(visitedIndexRooms.Count / 2, visitedIndexRooms.Count)];
        }
        keyPositions.Add(roomToChoose);
      }
    }

    private List<(int, int)> GenerateEnemySpawns(int[,] map, Random random, List<Room> rooms,
                                                 int difficulty) {
      List<(int, int)> enemySpawnList = new List<(int, int)>();
      for (int i = 0; i < difficulty; i++) {
        int roomToChoose = random.Next(1, rooms.Count);
        int enemyPosY = random.Next(rooms[roomToChoose].startPosY + 1, rooms[roomToChoose].endPosY);
        int enemyPosX = random.Next(rooms[roomToChoose].startPosX + 1, rooms[roomToChoose].endPosX);
        enemySpawnList.Add((enemyPosY, enemyPosX));
      }
      return enemySpawnList;
    }

    private List<(int, int)> GenerateItemSpawns(int[,] map, Random random, List<Room> rooms,
                                                int difficulty) {
      List<(int, int)> itemSpawnList = new List<(int, int)>();
      for (int i = 10; i > difficulty; i--) {
        int roomToChoose = random.Next(1, rooms.Count);
        int enemyPosY = random.Next(rooms[roomToChoose].startPosY + 1, rooms[roomToChoose].endPosY);
        int enemyPosX = random.Next(rooms[roomToChoose].startPosX + 1, rooms[roomToChoose].endPosX);
        itemSpawnList.Add((enemyPosY, enemyPosX));
      }
      return itemSpawnList;
    }
    private void AssembleMap(int[,] map, Random random, List<Room> rooms, List<Corridor> routes,
                             List<(Door, Door, int)> doorMST, List<int> keyPositions,
                             List<(int, int)> enemyPositionList,
                             List<(int, int)> itemPositionList) {
      PlaceRoomsOnMap(map, rooms);

      PlaceCorridorsOnMap(map, random, routes);

      PlaceDoorsOnMap(map, doorMST);

      PlaceKeysOnMap(map, rooms, random, keyPositions);

      PrepareEnemiesItemsSpawnPlaces(map, enemyPositionList, itemPositionList);

      PrepareEnterAndExit(map, random, rooms);
    }

    private void PlaceRoomsOnMap(int[,] map, List<Room> rooms) {
      foreach (Room room in rooms) {
        for (int y = room.startPosY; y <= room.endPosY; y++) {
          map[y, room.startPosX] = (int)MapCellStates.WALL;
          map[y, room.endPosX] = (int)MapCellStates.WALL;
        }
        for (int x = room.startPosX; x <= room.endPosX; x++) {
          map[room.startPosY, x] = (int)MapCellStates.WALL;
          map[room.endPosY, x] = (int)MapCellStates.WALL;
        }
        for (int x = room.startPosX + 1; x < room.endPosX; x++) {
          for (int y = room.startPosY + 1; y < room.endPosY; y++)
            map[y, x] = (int)MapCellStates.EMPTY;
        }
      }
    }

    private void PlaceCorridorsOnMap(int[,] map, Random random, List<Corridor> routes) {
      foreach (Corridor corridor in routes) {
        foreach (Tile tile in corridor.route.Tiles) {
          map[tile.PosY, tile.PosX] = (int)MapCellStates.CORRIDOR;
        }
        if (corridor.lockCode != 0) {
          int tileIndexToPick = random.Next(corridor.route.Tiles.Count);
          int doorPosY = corridor.route.Tiles[tileIndexToPick].PosY;
          int doorPosX = corridor.route.Tiles[tileIndexToPick].PosX;
          map[doorPosY, doorPosX] = (int)MapCellStates.DOOR + corridor.lockCode;
        }
      }
    }

    private void PlaceDoorsOnMap(int[,] map, List<(Door, Door, int)> doorMST) {
      foreach ((Door, Door, int)doorPair in doorMST) {
        for (int y = -1; y < 2; y++) {
          for (int x = -1; x < 2; x++) {
            if (map[y + doorPair.Item1.posY, x + doorPair.Item1.posX] == (int)MapCellStates.WALL &&
                Math.Abs(y + x) % 2 == 1) {
              map[y + doorPair.Item1.posY, x + doorPair.Item1.posX] = (int)MapCellStates.DOOR;
            }
            if (map[y + doorPair.Item2.posY, x + doorPair.Item2.posX] == (int)MapCellStates.WALL &&
                Math.Abs(y + x) % 2 == 1) {
              map[y + doorPair.Item2.posY, x + doorPair.Item2.posX] = (int)MapCellStates.DOOR;
            }
          }
        }
      }
    }

    private void PlaceKeysOnMap(int[,] map, List<Room> rooms, Random random,
                                List<int> keyPositions) {
      int key = 0;
      foreach (int keyPosition in keyPositions) {
        int keyPosY = random.Next(rooms[keyPosition].startPosY + 1, rooms[keyPosition].endPosY - 1);
        int keyPosX = random.Next(rooms[keyPosition].startPosX + 1, rooms[keyPosition].endPosX - 1);
        if (map[keyPosY, keyPosX] == (int)DoorLockState.OPEN) {
          key++;
          map[keyPosY, keyPosX] = key + (int)MapCellStates.KEY;
        }
      }
    }

    private void PrepareEnemiesItemsSpawnPlaces(int[,] map, List<(int, int)> enemyPositionList,
                                                List<(int, int)> itemPositionList) {
      foreach (var coordinate in enemyPositionList) {
        map[coordinate.Item1, coordinate.Item2] = (int)MapCellStates.ENEMY;
      }

      foreach (var coordinate in itemPositionList) {
        map[coordinate.Item1, coordinate.Item2] = (int)MapCellStates.ITEM;
      }
    }

    private void PrepareEnterAndExit(int[,] map, Random random, List<Room> rooms) {
      int enemyPosY = random.Next(rooms[0].startPosY + 1, rooms[0].endPosY);
      int enemyPosX = random.Next(rooms[0].startPosX + 1, rooms[0].endPosX);
      map[enemyPosY, enemyPosX] = (int)MapCellStates.ENTER;

      int roomToChoose = random.Next(1, rooms.Count);
      enemyPosY = random.Next(rooms[roomToChoose].startPosY + 1, rooms[roomToChoose].endPosY);
      enemyPosX = random.Next(rooms[roomToChoose].startPosX + 1, rooms[roomToChoose].endPosX);
      map[enemyPosY, enemyPosX] = (int)MapCellStates.EXIT;
    }

    public static double GetDistanceBetweenRooms(Room roomA, Room roomB) {
      return Math.Sqrt(Math.Pow(roomB.centerPosX - roomA.centerPosX, 2) +
                       Math.Pow(roomB.centerPosY - roomA.centerPosY, 2));
    }

    public static double GetDistanceBetweenCoords(int posYA, int posYB, int posXA, int posXB) {
      return Math.Sqrt(Math.Pow(posYB - posYA, 2) + Math.Pow(posXB - posXA, 2));
    }
  }
}
