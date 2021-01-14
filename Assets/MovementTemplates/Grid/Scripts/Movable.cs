using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] LayerMask blockingLayer;
    [SerializeField, Range(0.01f, 1f)] float movementSpeed = 0.04f;

    Vector3 targetPosition = Vector3.zero;

    void Start()
    {
        this.targetPosition = this.transform.position;
    }

    void FixedUpdate()
    {
        this.Move();
    }

    protected void SetTargetPosition(Vector3 direction)
    {
        var boxCast = Physics2D.Raycast(this.transform.position, direction, 1f, this.blockingLayer);

        if (boxCast || this.transform.position != this.targetPosition)
        {
            return;
        }

        this.targetPosition = this.transform.position + direction;
    }

    void Move()
    {
        if (this.transform.position != this.targetPosition)
        {
            this.transform.position =
                Vector3.MoveTowards(this.transform.position, this.targetPosition, this.movementSpeed);
        }
    }
}
