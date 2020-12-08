using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    enum MoveState
    {
        None,
        Left,
        Right,
        Jump
    }

    [SerializeField] LayerMask groundLayer;
    [SerializeField, Range(1, 200)] float horizontalAcceleration = 150f;
    [SerializeField, Range(1, 100)] float horizontalMaxSpeed = 10f;
    [SerializeField, Range(0, 2f)] float movementSmoothing = 0.01f;
    [SerializeField, Range(1, 100)] float jumpForce = 400f;
    [SerializeField, Range(0, 0.99f)] float slide = 0.8f;
    [SerializeField, Range(0.001f, 10)] float collisionBoxLength = 0.01f;

    Rigidbody2D rb2d;
    Collider2D collider2d;
    MoveState currentMoveState = MoveState.None;
    Vector2 currentVelocity = Vector2.zero;

    void Start()
    {
        this.rb2d = this.GetComponent<Rigidbody2D>();
        this.collider2d = this.GetComponent<Collider2D>();

        if (this.groundLayer.value == 0)
        {
            Debug.LogWarning(
                $"{nameof(Movement)}: It seems that the layer mask has not been set. Jumping might not work!");
        }
    }
    
    void Update()
    {
        this.currentMoveState = this.GetInput();
    }

    void FixedUpdate()
    {
        this.HandleJump();
        this.HandleHorizontalMovement();
    }

    void HandleHorizontalMovement()
    {
        var velocity = this.rb2d.velocity;
        var switchedDirection = (this.currentMoveState == MoveState.Left && velocity.x > 0f) ||
                                (this.currentMoveState == MoveState.Right && velocity.x < 0f);
        
        var result = switchedDirection ? 0f : velocity.x;

        switch (this.currentMoveState)
        {
            case MoveState.Left when result < -this.horizontalMaxSpeed:
                result -= this.horizontalAcceleration * Time.deltaTime;
                break;
            case MoveState.Left:
                result = -this.horizontalMaxSpeed;
                break;
            case MoveState.Right when result < this.horizontalMaxSpeed:
                result += this.horizontalAcceleration * Time.deltaTime;
                break;
            case MoveState.Right:
                result = this.horizontalMaxSpeed;
                break;
            case MoveState.Jump:
            case MoveState.None:
            default:
                result *= this.slide;
                break;
        }

        this.rb2d.velocity = Vector2.SmoothDamp(
            current: velocity,
            target: new Vector2(result, velocity.y),
            currentVelocity: ref this.currentVelocity,
            smoothTime: this.movementSmoothing);
    }

    void HandleJump()
    {
        if (this.currentMoveState != MoveState.Jump || !this.IsColliding(Vector2.down)) return;
        
        this.rb2d.AddForce(new Vector2(0f, this.jumpForce * this.rb2d.gravityScale), ForceMode2D.Impulse);
        
        this.currentMoveState = MoveState.None;
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

    MoveState GetInput()
    {
        if (Input.GetKey(KeyCode.Space) &&
            this.currentMoveState != MoveState.Jump &&
            this.IsColliding(Vector2.down))
        {
            return MoveState.Jump;
        }
        
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
        {
            return MoveState.Right;
        }
        
        return horizontalInput < 0 ? MoveState.Left : MoveState.None;
    }
}
