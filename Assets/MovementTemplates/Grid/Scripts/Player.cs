using UnityEngine;

public class Player : Movable
{
    void Update()
    {
        var moveDirection = this.GetMoveDirection();

        if (moveDirection != Vector3.zero)
        {
            this.SetTargetPosition(moveDirection);
        }
    }

    Vector3 GetMoveDirection()
    {
        if (Input.GetKey(KeyCode.A))
        {
            return Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            return Vector3.right;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            return Vector3.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            return Vector3.down;
        }

        return Vector3.zero;
    }
}
