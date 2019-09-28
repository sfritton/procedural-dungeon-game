using System;
using UnityEngine;

public class Door
{
  public static int DOOR_WIDTH = 2;
  public Room From, To;
  // private bool isOnPath = false;
  // private bool isLocked = false;
  public int X1, X2, Y1, Y2;
  public Direction direction = Direction.NONE;
  private Boolean isVertical = false;

  public Door(Room from, Room to)
  {
    this.From = from;
    this.To = to;

    Direction direction = from.IsNeighbor(to);
    this.direction = direction;

    // use the direction to set isVertical and the secondary axis
    switch (direction)
    {
      case Direction.NORTH:
        this.isVertical = false;
        this.Y1 = from.Y1 - 1;
        this.Y2 = from.Y1;
        break;
      case Direction.EAST:
        this.isVertical = true;
        this.X1 = from.X2;
        this.X2 = from.X2 + 1;
        break;
      case Direction.SOUTH:
        this.isVertical = false;
        this.Y1 = from.Y2;
        this.Y2 = from.Y2 + 1;
        break;
      case Direction.WEST:
        this.isVertical = true;
        this.X1 = from.X1 - 1;
        this.X2 = from.X1;
        break;
    }

    int halfWidth = DOOR_WIDTH / 2;

    if (isVertical)
    {
      int a1 = Math.Max(from.Y1, to.Y1);
      int a2 = Math.Min(from.Y2, to.Y2);
      int a = (int)UnityEngine.Random.Range(a1 + halfWidth + 1, a2 - halfWidth - 1);
      this.Y1 = a - halfWidth;
      this.Y2 = a + halfWidth;
    }
    else
    {
      int a1 = Math.Max(from.X1, to.X1);
      int a2 = Math.Min(from.X2, to.X2);
      int a = (int)UnityEngine.Random.Range(a1 + halfWidth + 1, a2 - halfWidth - 1);
      this.X1 = a - halfWidth;
      this.X2 = a + halfWidth;
    }
  }

  public void Bake(GameObject wallPrefab, GameObject floorPrefab, float wallHeight)
  {
    GameObject door = new GameObject("Door");
    GameObject floor = GameObject.Instantiate(floorPrefab);
    GameObject wall1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
    GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Quad);

    float centerX = ((float)(this.X1 + this.X2)) / 2;
    float centerY = ((float)(this.Y1 + this.Y2)) / 2;

    floor.transform.parent = door.transform;
    wall1.transform.parent = door.transform;
    wall2.transform.parent = door.transform;

    wall1.transform.position = new Vector3(0, 0.5f, 0.5f);

    wall2.transform.Rotate(new Vector3(0, -180, 0));
    wall2.transform.position = new Vector3(0, 0.5f, -0.5f);

    door.transform.position = new Vector3(centerX, 0, centerY);
    door.transform.localScale = new Vector3(1, wallHeight, DOOR_WIDTH);

    if (!isVertical)
    {
      door.transform.Rotate(new Vector3(0, 90, 0));
    }

    this.To.Bake(wallPrefab, floorPrefab);
  }
}
