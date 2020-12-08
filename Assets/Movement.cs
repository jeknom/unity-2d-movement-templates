using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    enum MoveState
    {
        None,
        Left,
        Right
    }

    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 acceleration = new Vector2(150f, -200f);
    [SerializeField, Range(1, 100)] float horizontalMaxSpeed = 10f;
    [SerializeField, Range(1, 200)] float verticalMaxSpeed = 30f;
    [SerializeField, Range(0, 0.99f)] float slide = 0.8f;
    [SerializeField, Range(0, 0.99f)] float collisionBoxLength = 0.01f;

    Rigidbody2D rb2d;
    Collider2D collider2d;
    MoveState currentState = MoveState.None;

    void Start()
    {
        this.rb2d = this.GetComponent<Rigidbody2D>();
        this.collider2d = this.GetComponent<Collider2D>();

        if (this.groundLayer.value == 0)
        {
            Debug.LogWarning(
                $"{nameof(Movement)}: It seems that the layer mask has not been set. Collisions might not work!");
        }
    }
    
    void Update()
    {
        this.currentState = GetInput();
    }

    void FixedUpdate()
    {
        var currentVelocity = this.rb2d.velocity;
        var collisions = this.GetCollisions();
        
        var horizontalVelocity = GetHorizontalVelocity(
            this.currentState,
            this.acceleration,
            collisions,
            currentVelocity.x,
            this.slide,
            this.horizontalMaxSpeed);
        var verticalVelocity = GetVerticalVelocity(
            this.acceleration,
            collisions,
            currentVelocity.y,
            this.verticalMaxSpeed);

        this.rb2d.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    static float GetHorizontalVelocity(
        MoveState currentState,
        Vector2 acceleration,
        HashSet<Vector2> collisions,
        float currentVelocity,
        float slide,
        float maxSpeed)
    {
        var switchedDirection = (currentState == MoveState.Left && currentVelocity > 0f) ||
                                (currentState == MoveState.Right && currentVelocity < 0f);
        var result = switchedDirection ? 0f : currentVelocity;

        switch (currentState)
        {
            case MoveState.Left when collisions.Contains(Vector2.left):
            case MoveState.Right when collisions.Contains(Vector2.right):
                result = 0f;
                break;
            case MoveState.Left when result < -maxSpeed:
                result -= acceleration.x * Time.deltaTime;
                break;
            case MoveState.Left:
                result = -maxSpeed;
                break;
            case MoveState.Right when result < maxSpeed:
                result += acceleration.x * Time.deltaTime;
                break;
            case MoveState.Right:
                result = maxSpeed;
                break;
            case MoveState.None:
            default:
                result *= slide;
                break;
        }

        return result;
    }

    static float GetVerticalVelocity(
        Vector2 acceleration,
        HashSet<Vector2> collisions,
        float currentVelocity,
        float maxSpeed)
    {
        var isColliding = collisions.Contains(Vector2.up) || collisions.Contains(Vector2.down);
        var result = currentVelocity;
        
        if (result > -maxSpeed)
        {
            result += acceleration.y * Time.deltaTime;
        }
        else
        {
            result = acceleration.y * Time.deltaTime;
        }

        return isColliding ? 0f : result;
    }

    HashSet<Vector2> GetCollisions()
    {
        var set = new HashSet<Vector2>();

        if (this.IsColliding(Vector2.up))
        {
            set.Add(Vector2.up);
        }
        
        if (this.IsColliding(Vector2.right))
        {
            set.Add(Vector2.right);
        }

        if (this.IsColliding(Vector2.left))
        {
            set.Add(Vector2.left);
        }

        if (this.IsColliding(Vector2.down))
        {
            set.Add(Vector2.down);
        }

        return set;
    }

    bool IsColliding(Vector2 direction)
    {
        var playerBounds = this.collider2d.bounds;
        var hit = Physics2D.BoxCast(
            origin: playerBounds.center,
            // Don't want to include the corners so only getting 90% of the box.
            size: playerBounds.size * 0.90f,
            angle: 0f,
            direction: direction,
            distance: this.collisionBoxLength,
            layerMask: this.groundLayer);

        return hit.collider != null;
    }

    static MoveState GetInput()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
        {
            return MoveState.Right;
        }
        
        return horizontalInput < 0 ? MoveState.Left : MoveState.None;
    }
}
