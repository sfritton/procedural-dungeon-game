using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
  public float MULTIPLIER = 0.25f;
  public int DUNGEON_WIDTH = 80;
  public int DUNGEON_HEIGHT = 80;
  public int DEPTH = 5;
  public GameObject player;
  public GameObject wall;
  public GameObject floor;

  private List<Room> rooms;
  private Room dungeonEntrance;
  private Room dungeonExit;

  // Start is called before the first frame update
  void Start()
  {
    GenerateDungeon();

    Vector3 spawnPoint = new Vector3((dungeonEntrance.X1 + dungeonEntrance.X2) / 2, 1f, (dungeonEntrance.Y1 + dungeonEntrance.Y2) / 2);

    GameObject.Instantiate(player, spawnPoint, Quaternion.identity);
  }

  void GenerateDungeon()
  {
    rooms = new List<Room>(); // clear the previous dungeon

    // divide the dungeon into rooms
    Divide(DEPTH);

    // pick an entrance
    dungeonEntrance = rooms[(int)Math.Round((double)UnityEngine.Random.Range(0, rooms.Count - 1))];
    dungeonEntrance.IsDungeonEntrance = true;

    // connect those rooms with doors, storing the longest path
    dungeonEntrance.FindExits(rooms);

    // pick an exit
    dungeonEntrance.FindDungeonExit();

    // render the dungeon
    dungeonEntrance.Bake(wall, floor);
  }

  void Divide(int depth)
  {
    Room root = new Room(0, DUNGEON_WIDTH, 0, DUNGEON_HEIGHT);
    Divide(depth, root);
  }

  void Divide(int depth, Room room)
  {
    if (depth == 0)
    {
      rooms.Add(room);
      return;
    }

    bool isVertical = depth % 2 == 0;
    bool flip = UnityEngine.Random.value > .85; // 15% of the time, break from the normal pattern

    if (flip ? !isVertical : isVertical)
    {
      int middleX = RandomMiddle(room.X1, room.X2);

      if (room.X2 - (middleX + 1) < Room.MIN_WALL_LENGTH || middleX - room.X1 < Room.MIN_WALL_LENGTH)
      {
        Divide(depth - 1, room);
        return;
      }

      Room leftRoom = new Room(room.X1, middleX, room.Y1, room.Y2);
      Room rightRoom = new Room(middleX + 1, room.X2, room.Y1, room.Y2);

      Divide(depth - 1, leftRoom);
      Divide(depth - 1, rightRoom);

      return;
    }

    int middleY = RandomMiddle(room.Y1, room.Y2);

    if (room.Y2 - (middleY + 1) < Room.MIN_WALL_LENGTH || middleY - room.Y1 < Room.MIN_WALL_LENGTH)
    {
      Divide(depth - 1, room);
      return;
    }

    Room topRoom = new Room(room.X1, room.X2, room.Y1, middleY);
    Room bottomRoom = new Room(room.X1, room.X2, middleY + 1, room.Y2);

    Divide(depth - 1, topRoom);
    Divide(depth - 1, bottomRoom);
  }

  int RandomMiddle(int min, int max)
  {
    float middlePercent = (0.5f + UnityEngine.Random.Range(-MULTIPLIER, MULTIPLIER));
    float middleFloat = min + ((max - min) * middlePercent);

    return (int)Math.Round(middleFloat);
  }
}
