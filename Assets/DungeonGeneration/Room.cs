using System.Collections.Generic;
using UnityEngine;


public class Room
{
  public static int MIN_WALL_LENGTH = 5;
  public static float WALL_HEIGHT = 2f;
  public int X1 = 0;
  public int X2 = 0;
  public int Y1 = 0;
  public int Y2 = 0;
  public List<Door> Exits = new List<Door>();
  public Door Entrance;
  public int Depth = 0;
  public int MaxDepth = 0;
  public bool IsDungeonEntrance = false;
  public bool IsDungeonExit = false;

  public Room(int x1, int x2, int y1, int y2)
  {
    this.X1 = x1;
    this.X2 = x2;
    this.Y1 = y1;
    this.Y2 = y2;
  }

  public void Bake(GameObject wallPrefab, GameObject floorPrefab)
  {
    GameObject room = new GameObject("Room");
    GameObject floor = GameObject.Instantiate(floorPrefab);

    float centerX = ((float)(this.X1 + this.X2)) / 2;
    float centerY = ((float)(this.Y1 + this.Y2)) / 2;
    float width = ((float)(this.X2 - this.X1));
    float height = ((float)(this.Y2 - this.Y1));

    List<int> northVertices = new List<int>();
    List<int> southVertices = new List<int>();
    List<int> eastVertices = new List<int>();
    List<int> westVertices = new List<int>();

    northVertices.Add(this.X1);
    northVertices.Add(this.X2);

    southVertices.Add(this.X1);
    southVertices.Add(this.X2);

    eastVertices.Add(this.Y1);
    eastVertices.Add(this.Y2);

    westVertices.Add(this.Y1);
    westVertices.Add(this.Y2);

    this.Exits.ForEach(exit =>
    {
      switch (exit.direction)
      {
        case Direction.NORTH:
          northVertices.Add(exit.X1);
          northVertices.Add(exit.X2);
          break;
        case Direction.SOUTH:
          southVertices.Add(exit.X1);
          southVertices.Add(exit.X2);
          break;
        case Direction.EAST:
          eastVertices.Add(exit.Y1);
          eastVertices.Add(exit.Y2);
          break;
        case Direction.WEST:
          westVertices.Add(exit.Y1);
          westVertices.Add(exit.Y2);
          break;
      }
    });

    if (this.Entrance != null)
    {
      switch (this.Entrance.direction)
      {
        case Direction.SOUTH:
          northVertices.Add(this.Entrance.X1);
          northVertices.Add(this.Entrance.X2);
          break;
        case Direction.NORTH:
          southVertices.Add(this.Entrance.X1);
          southVertices.Add(this.Entrance.X2);
          break;
        case Direction.WEST:
          eastVertices.Add(this.Entrance.Y1);
          eastVertices.Add(this.Entrance.Y2);
          break;
        case Direction.EAST:
          westVertices.Add(this.Entrance.Y1);
          westVertices.Add(this.Entrance.Y2);
          break;
      }
    }

    northVertices.Sort();
    southVertices.Sort();
    eastVertices.Sort();
    westVertices.Sort();

    for (int i = 1; i < northVertices.Count; i += 2)
    {
      float wallCenter = ((float)(northVertices[i - 1] + northVertices[i])) / 2;
      float wallWidth = ((float)(northVertices[i] - northVertices[i - 1]));

      GameObject wall = GameObject.Instantiate(wallPrefab);

      wall.transform.parent = room.transform;

      wall.transform.position = new Vector3(wallCenter - centerX, 0.5f, (centerY - this.Y2));
      wall.transform.localScale = new Vector3(wallWidth, 1, 1);
      wall.transform.Rotate(new Vector3(0, 180, 0));
    }

    for (int i = 1; i < southVertices.Count; i += 2)
    {
      float wallCenter = ((float)(southVertices[i - 1] + southVertices[i])) / 2;
      float wallWidth = ((float)(southVertices[i] - southVertices[i - 1]));

      GameObject wall = GameObject.Instantiate(wallPrefab);

      wall.transform.parent = room.transform;

      wall.transform.position = new Vector3(wallCenter - centerX, 0.5f, (centerY - this.Y1));
      wall.transform.localScale = new Vector3(wallWidth, 1, 1);
    }

    for (int i = 1; i < eastVertices.Count; i += 2)
    {
      float wallCenter = ((float)(eastVertices[i - 1] + eastVertices[i])) / 2;
      float wallWidth = ((float)(eastVertices[i] - eastVertices[i - 1]));

      GameObject wall = GameObject.Instantiate(wallPrefab);

      wall.transform.parent = room.transform;

      wall.transform.position = new Vector3(centerX - this.X1, 0.5f, wallCenter - centerY);
      wall.transform.localScale = new Vector3(wallWidth, 1, 1);
      wall.transform.Rotate(new Vector3(0, 90, 0));
    }

    for (int i = 1; i < westVertices.Count; i += 2)
    {
      float wallCenter = ((float)(westVertices[i - 1] + westVertices[i])) / 2;
      float wallWidth = ((float)(westVertices[i] - westVertices[i - 1]));

      GameObject wall = GameObject.Instantiate(wallPrefab);

      wall.transform.parent = room.transform;

      wall.transform.position = new Vector3(centerX - this.X2, 0.5f, wallCenter - centerY);
      wall.transform.localScale = new Vector3(wallWidth, 1, 1);
      wall.transform.Rotate(new Vector3(0, -90, 0));
    }

    floor.transform.parent = room.transform;
    floor.transform.localScale = new Vector3(width, 1, height);

    room.transform.position = new Vector3(centerX, 0, centerY);
    room.transform.localScale = new Vector3(1, WALL_HEIGHT, 1);

    this.Exits.ForEach(exit => exit.Bake(wallPrefab, floorPrefab, WALL_HEIGHT));
  }

  public Direction IsNeighbor(Room other)
  {
    // north
    if (this.Y1 == other.Y2 + 1 && (other.X2 - this.X1 > MIN_WALL_LENGTH) && (this.X2 - other.X1 > MIN_WALL_LENGTH)) return Direction.NORTH;

    // east
    if (this.X2 == other.X1 - 1 && (other.Y2 - this.Y1 > MIN_WALL_LENGTH) && (this.Y2 - other.Y1 > MIN_WALL_LENGTH)) return Direction.EAST;

    // south
    if (this.Y2 == other.Y1 - 1 && (other.X2 - this.X1 > MIN_WALL_LENGTH) && (this.X2 - other.X1 > MIN_WALL_LENGTH)) return Direction.SOUTH;

    // west
    if (this.X1 == other.X2 + 1 && (other.Y2 - this.Y1 > MIN_WALL_LENGTH) && (this.Y2 - other.Y1 > MIN_WALL_LENGTH)) return Direction.WEST;

    return Direction.NONE;
  }

  public int FindExits(List<Room> candidates)
  {
    this.MaxDepth = this.Depth;

    if (candidates.Count == 0) return this.Depth;

    candidates.ForEach(candidate =>
    {
      Direction direction = this.IsNeighbor(candidate);

      if (
          direction == Direction.NONE || // ignore non-neighbors
          candidate.Entrance != null || candidate.IsDungeonEntrance || // ignore candidates that have already been claimed
          UnityEngine.Random.value > .85 // ignore 15% of valid connections
        )
      {
        return;
      }

      Door door = new Door(this, candidate);

      this.Exits.Add(door);
      candidate.Entrance = door;
      candidate.Depth = this.Depth + 1;
    });

    this.Exits.ForEach(exit =>
    {
      int exitDepth = exit.To.FindExits(candidates);
      if (exitDepth > this.MaxDepth) this.MaxDepth = exitDepth;
    });

    return this.MaxDepth;
  }

  public Room FindMaxDepthDescendent()
  {
    if (this.Exits.Count == 0) return this;

    Room maxChild = this.Exits[0].To;

    this.Exits.ForEach(exit =>
    {
      Room candidate = exit.To;
      if (candidate.MaxDepth > maxChild.MaxDepth)
      {
        maxChild = candidate;
      }
    });

    return maxChild.FindMaxDepthDescendent();
  }

  public Room FindDungeonExit()
  {
    if (!this.IsDungeonEntrance) return null;

    Room exit = this.FindMaxDepthDescendent();
    exit.IsDungeonExit = true;
    return exit;
  }

  //   public void findPath()
  //   {
  //     if (!this.isDungeonExit) return;

  //     Door entrance = this.entrance;

  //     while (entrance != null)
  //     {
  //       entrance.isOnPath = true;
  //       entrance.from.isOnPath = true;

  //       entrance = entrance.from.entrance;
  //     }
  //   }

  //   private Door findNonPathExit()
  //   {
  //     Door maxDepthNonPathExit = null;

  //     foreach (var exit in this.exits)
  //     {
  //       if (exit.isOnPath) continue;

  //       if (maxDepthNonPathExit == null || exit.to.maxDepth > maxDepthNonPathExit.to.maxDepth)
  //       {
  //         maxDepthNonPathExit = exit;
  //       }
  //     }

  //     return maxDepthNonPathExit;
  //   }

  //   private Door findPathExit()
  //   {
  //     foreach (var exit in this.exits)
  //     {
  //       if (exit.isOnPath) return exit;
  //     }

  //     return null;
  //   }

  //   // return true if a key was successfully placed, false otherwise
  //   public bool placeKey()
  //   {
  //     if (!this.isDungeonExit) return false;

  //     int distance = random(1, this.depth - 1);

  //     Room room = this;

  //     // walk up the tree for distance levels
  //     for (int i = distance; i > 0 && room.entrance != null; i--)
  //     {
  //       room = room.entrance.from;
  //     }

  //     Door nonPathExit = null;

  //     while (nonPathExit == null)
  //     {
  //       nonPathExit = room.findNonPathExit();

  //       if (nonPathExit == null)
  //       {
  //         if (room.entrance == null) return false;

  //         // walk up another level
  //         room = room.entrance.from;
  //       }
  //     }

  //     // place the lock
  //     room.findPathExit().isLocked = true;

  //     Room keyRoom = nonPathExit.to.findMaxDepthDescendent();

  //     // place the key
  //     keyRoom.hasKey = true;

  //     return true;
  //   }
}
