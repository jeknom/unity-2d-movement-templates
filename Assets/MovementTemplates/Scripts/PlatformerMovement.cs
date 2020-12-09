using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace MovementTemplates.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlatformerMovement : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] LayerMask groundLayer;
        
        [Header("Keys")]
        [SerializeField] KeyCode moveLeftKey = KeyCode.A;
        [SerializeField] KeyCode moveRightKey = KeyCode.D;
        [SerializeField] KeyCode jumpKey = KeyCode.Space;
        [SerializeField] KeyCode moveFastKey = KeyCode.LeftShift;
        [SerializeField] KeyCode moveSlowKey = KeyCode.C;
        
        [Header("Modifiers")]
        [SerializeField, Range(1, 500)] float horizontalAcceleration = 150f;
        [SerializeField, Range(1, 500)] float horizontalMaxSpeed = 15f;
        [SerializeField, Range(1, 500)] float horizontalMoveFastMaxSpeed = 20f;
        [SerializeField, Range(1, 500)] float horizontalMoveSlowMaxSpeed = 5f;
        [SerializeField, Range(1, 100)] float jumpForce = 4.5f;
        [SerializeField, Range(0, 1f)] float slide = 0.5f;
        [SerializeField, Range(0, 1f)] float movementSmoothing = 0.01f;
        [SerializeField, Range(0.1f, 1f)] float collisionBoxLength = 0.1f;

        Rigidbody2D rb2d;
        Collider2D collider2d;
        KeyCode currentInput = KeyCode.None;
        Vector2 currentVelocity = Vector2.zero;
        // Need this flag as FixedUpdate() might not pick up on quick tap of the jump key.
        bool wasJumpPressed;
        bool isMovingFast;
        bool isMovingSlow;

        void Start()
        {
            this.rb2d = this.GetComponent<Rigidbody2D>();
            this.collider2d = this.GetComponent<Collider2D>();

            if (this.groundLayer.value == 0)
            {
                Debug.LogWarning(
                    $"{nameof(PlatformerMovement)}: It seems that the layer mask has not been set. Jumping might not work!");
            }
        }
    
        void Update()
        {
            this.GetInput();
        }

        void FixedUpdate()
        {
            this.HandleJump();
            this.HandleHorizontalMovement();
        }

        void HandleHorizontalMovement()
        {
            var velocity = this.rb2d.velocity;
            var switchedDirection = (this.currentInput == this.moveLeftKey && velocity.x > 0f) ||
                                    (this.currentInput == this.moveRightKey && velocity.x < 0f);
        
            var result = switchedDirection ? 0f : velocity.x;
            var maxSpeedToUse = this.isMovingFast ?
                this.horizontalMoveFastMaxSpeed :
                this.isMovingSlow ?
                this.horizontalMoveSlowMaxSpeed :
                this.horizontalMaxSpeed;

            if (this.currentInput == this.moveLeftKey && result > -maxSpeedToUse)
            {
                result -= this.horizontalAcceleration * Time.deltaTime;
            }
            else if (this.currentInput == this.moveLeftKey)
            {
                result = -maxSpeedToUse;
            }
            else if (this.currentInput == this.moveRightKey && result < maxSpeedToUse)
            {
                result += this.horizontalAcceleration * Time.deltaTime;
            }
            else if (this.currentInput == this.moveRightKey)
            {
                result = maxSpeedToUse;
            }
            else
            {
                result *= this.slide;
            }

            this.rb2d.velocity = Vector2.SmoothDamp(
                current: velocity,
                target: new Vector2(result, velocity.y),
                currentVelocity: ref this.currentVelocity,
                smoothTime: this.movementSmoothing);
        }

        void HandleJump()
        {
            if (!this.wasJumpPressed) return;

            this.wasJumpPressed = false;

            if (this.IsColliding(Vector2.down))
            {
                this.rb2d.AddForce(new Vector2(0f, this.jumpForce * this.rb2d.gravityScale), ForceMode2D.Impulse);
            }
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

        void GetInput()
        {
            this.isMovingFast = Input.GetKey(this.moveFastKey);
            this.isMovingSlow = Input.GetKey(this.moveSlowKey);
            this.wasJumpPressed = Input.GetKey(this.jumpKey);
            
            if (Input.GetKey(this.moveRightKey))
            {
                this.currentInput = this.moveRightKey;
            }
            else if (Input.GetKey(this.moveLeftKey))
            {
                this.currentInput = this.moveLeftKey;
            }
            else
            {
                this.currentInput = KeyCode.None;
            }
        }
    }
}
