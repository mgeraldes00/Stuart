using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float jumpSpeed = 5.0f;
    public float JumpSpeed => jumpSpeed;
    [SerializeField] private Transform groundProbe;
    [SerializeField] private float groundProbeRadius = 5.0f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private int defaultLayer = 3;
    [SerializeField] private float maxJumpTime = 0.1f;
    [SerializeField] private float fallGravityScale = 5.0f;
    [SerializeField] private Vector3 currentVelocity;
    public Vector3 CurrentVelocity => currentVelocity;
    [SerializeField] private TimeUpdater timeUpdater;

    private bool isInputLocked => (inputLockTimer > 0);

    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    [SerializeField] private float jumpTime;
    [SerializeField] private float inputLockTimer = 0;
    [SerializeField] private bool onGround;
    public bool OnGround => onGround;
    [SerializeField] private bool onPlatform;
    public bool OnPlatform => onPlatform;
    [SerializeField] private bool jumping;
    [SerializeField] private bool gliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        currentVelocity = rb.velocity;
        onGround = IsOnGround();
        onPlatform = IsOnPlatform();

        if (isInputLocked)
        {
            inputLockTimer -= Time.deltaTime;
            if (timeUpdater)
            {
                timeUpdater.SetScale(1.0f);
            }
        }
        else
        {
            float hAxis = Input.GetAxis("Horizontal");
            currentVelocity.x = hAxis * horizontalSpeed;

            if (timeUpdater != null)
            {
                if (Mathf.Abs(hAxis) > 0) timeUpdater.SetScale(1.0f);
                else timeUpdater.SetScale(0.0f);
            }

            if (onPlatform)
            {
                // BUG : sometimes disables glide incorrectly when touching
                // a platform but not standing on it
                jumping = false;
                gliding = false;
                sprite.sortingOrder = defaultLayer - 2;
            }
            else if (onGround)
            {
                jumping = false;
                gliding = false;
                sprite.sortingOrder = defaultLayer;
            }
            
            if (Input.GetButtonDown("Jump"))
            {
                if (onGround || onPlatform)
                {
                    rb.gravityScale = 1.0f;
                    currentVelocity.y = jumpSpeed;
                    jumpTime = Time.time;
                }
                else
                {
                    if (currentVelocity.y <= 0)
                    {
                        gliding = true;
                        rb.gravityScale = 0.2f;
                    }
                }
            }
            else if (Input.GetButton("Jump"))
            {
                float elapsedTime = Time.time - jumpTime;
                if (elapsedTime > maxJumpTime && !gliding)
                {
                    rb.gravityScale = fallGravityScale;
                }

                if (currentVelocity.y > 0 && !onPlatform && !onGround)
                    jumping = true;
                if (jumping && currentVelocity.y <= 0)
                {
                    gliding = true;
                    rb.gravityScale = 0.2f;
                }
            }
            else if (Input.GetButtonUp("Jump"))
            {
                jumping = false;

                if (gliding)
                    gliding = false;
            }
            else
            {
                if (!gliding)
                    rb.gravityScale = fallGravityScale;
            }

            rb.velocity = currentVelocity;

            if ((currentVelocity.x > 0) && (transform.right.x < 0))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if ((currentVelocity.x < 0) && (transform.right.x > 0))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private bool IsOnGround()
    {
        var collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius, groundMask);

        return (collider != null);
    }

    private bool IsOnPlatform()
    {
        var collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius, platformMask);

        return (collider != null);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundProbe != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundProbe.position, groundProbeRadius);
        }
    }
}
