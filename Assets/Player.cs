using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

  public float speed = 6f; // The speed that the player will move at.
  public GameObject playerMesh; // Reference to the player's mesh.

  Vector3 movement; // The vector to store the direction of the player's movement.
  Rigidbody playerRigidbody; // Reference to the player's rigidbody.

  void Awake()
  {
    playerRigidbody = GetComponent<Rigidbody>();
  }


  void FixedUpdate()
  {
    // Store the input axes.
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");

    // Move the player around the scene.
    Move(h, v);
  }

  void Move(float h, float v)
  {
    // Set the movement vector based on the axis input.
    movement.Set(h, 0f, v);

    // Make the player face the direction it's heading.
    Turn();

    // Normalise the movement vector and make it proportional to the speed per second.
    movement = movement.normalized * speed * Time.deltaTime;

    // Move the player to it's current position plus the movement.
    playerRigidbody.MovePosition(transform.position + movement);
  }

  void Turn()
  {
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
      playerMesh.transform.forward = new Vector3(0f, 0f, 1f);
    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
      playerMesh.transform.forward = new Vector3(0f, 0f, -1f);
    else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
      playerMesh.transform.forward = new Vector3(-1f, 0f, 0f);
    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
      playerMesh.transform.forward = new Vector3(1f, 0f, 0f);
  }

}
