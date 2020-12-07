using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    enum MoveState
    {
        None,
        Left,
        Right
    }
    
    [SerializeField] Vector2 acceleration = new Vector2(150f, -100f);
    [SerializeField, Range(1, 100)] float horizontalMaxSpeed = 10f;
    [SerializeField, Range(0, 0.99f)] float slide = 0.8f;
    [SerializeField] Rigidbody2D rb2d;

    MoveState currentState = MoveState.None;

    void Start()
    {
        this.rb2d = this.GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        this.currentState = GetInput();
    }

    void FixedUpdate()
    {
        var horizontalVelocity = GetHorizontalVelocity(
            this.currentState,
            this.acceleration,
            this.rb2d.velocity.x,
            this.slide,
            this.horizontalMaxSpeed);
        
        this.rb2d.velocity = new Vector2(horizontalVelocity, 0f);
    }

    static float GetHorizontalVelocity(
        MoveState currentState,
        Vector2 acceleration,
        float currentVelocity,
        float slide,
        float maxSpeed)
    {
        var switchedDirection = (currentState == MoveState.Left && currentVelocity > 0f) ||
                                (currentState == MoveState.Right && currentVelocity < 0f);
        var result = switchedDirection ? 0f : currentVelocity;

        switch (currentState)
        {
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
