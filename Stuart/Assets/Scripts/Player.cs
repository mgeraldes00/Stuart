using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float glideCooldownAmount = 0.5f;

    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField] private Transform follower;
    [SerializeField] private Transform groundProbe;
    [SerializeField] private Transform platformProbeFront, platformProbeBack;
    [SerializeField] private Transform enterPoint, leavePoint;
    [SerializeField] private float groundProbeRadius = 5.0f;
    [SerializeField] private float platformProbeRadius = 1.0f;
    [SerializeField] private Vector2 groundProbeSize;
    [SerializeField] private LayerMask groundMask, platformMask, edgeMask;
    [SerializeField] private LayerMask boundMask, coinMask;
    [SerializeField] private int defaultLayer = 3;
    [SerializeField] private float maxJumpTime = 0.1f;
    [SerializeField] private float fallGravityScale = 5.0f;
    [SerializeField] private Vector3 currentVelocity;
    public Vector3 CurrentVelocity => currentVelocity;
    [SerializeField] private TimeUpdater timeUpdater;

    [SerializeField] private float time;

    private bool isInputLocked => (inputLockTimer > 0);

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    [SerializeField] private BoxCollider2D platformCollider;

    [SerializeField] private SpeechBalloon speechBalloon;

    [SerializeField] private AudioSource[] sounds;

    private float jumpTime;
    [SerializeField] private float glideCooldown;
    [SerializeField] private float inputLockTimer = 0;
    [SerializeField] private bool onGround;
    public bool OnGround => onGround;
    [SerializeField] private bool onPlatform, platforming, hoveringPlatform;
    public bool OnPlatform => onPlatform;
    public bool Platforming => platforming;
    public bool HoveringPlatform => hoveringPlatform;
    [SerializeField] private bool onBound, onCoin;
    [SerializeField] private bool balancingFront, balancingBack;
    [SerializeField] private bool jumping;
    public bool Jumping => jumping;
    [SerializeField] private bool gliding;
    public bool Gliding => gliding;
    [SerializeField] private bool requestingDown, colliderDecrease;
    [SerializeField] private bool buttonHold;

    [SerializeField] private bool isLocked;
    public bool IsLocked => isLocked;

    [SerializeField] private bool enteredScene;

    [SerializeField] private bool hasPlayed, canLand;

    private bool talking, turning;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        speechBalloon = GetComponentInChildren<SpeechBalloon>();

        follower = GameObject.FindWithTag("Follower").GetComponent<Transform>();

        onGround = true;
    }

    private void Update()
    {
        currentVelocity = rb.velocity;
        onGround = IsOnGround();
        onPlatform = IsOnPlatform();
        onBound = IsOnBound();
        onCoin = IsOnCoin();
        hoveringPlatform = IsHovering();
        balancingBack = IsBalancingBack();
        balancingFront = IsBalancingFront();

        if (onPlatform)
        {
            sprite.sortingOrder = defaultLayer - 2;
            platforming = true;
        }
            
        if (onGround)
        {
            sprite.sortingOrder = defaultLayer;
            platforming = false;
        }

        if (enteredScene && !isLocked)
        {
            UpdateMovement();
        }

        animator.SetFloat("VelocityX", Mathf.Abs(currentVelocity.x));
        animator.SetFloat("VelocityY", currentVelocity.y);
        animator.SetFloat("Gravity", rb.gravityScale);
        animator.SetFloat("GlideCooldown", glideCooldown);
        animator.SetBool("grounded", onGround);
        animator.SetBool("onPlatform", onPlatform);
        animator.SetBool("gliding", gliding);
        animator.SetBool("balancingLeft", balancingBack);
        animator.SetBool("balancingRight", balancingFront);
    }

    private void UpdateMovement()
    {
        if (isInputLocked)
        {
            inputLockTimer -= Time.deltaTime;
            if (timeUpdater) timeUpdater.SetScale(1.0f);
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

            if (onPlatform || onGround)
            {
                jumping = false;
                gliding = false;
                hasPlayed = false;
                glideCooldown = 0;
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
                    if (currentVelocity.y <= -5f && glideCooldown <= 0)
                    {
                        Glide();

                        if (currentVelocity.y < -0.8f)
                            currentVelocity.y -= currentVelocity.y * 0.8f;

                        sounds[0].Play();
                        glideCooldown = glideCooldownAmount;
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

                if (currentVelocity.y >= 0 && !onPlatform && !onGround)
                {
                    jumping = true;
                }
                if (jumping && currentVelocity.y <= 0
                    || !jumping && currentVelocity.y <= -5f && glideCooldown <= 0 
                    && !onGround && !onPlatform)
                {
                    Glide();

                    buttonHold = true;

                    if (!hasPlayed)
                    {
                        if (currentVelocity.y < -0.8f)
                            currentVelocity.y -= currentVelocity.y * 0.8f;

                        sounds[0].Play();
                        glideCooldown = glideCooldownAmount;
                    }
                    
                    hasPlayed = true;
                }
            }
            else if (Input.GetButtonUp("Jump"))
            {
                jumping = false;
                buttonHold = false;

                if (gliding)
                {
                    gliding = false;
                    StartCoroutine(ResetGlideCooldown());
                }
            }
            else
            {
                if (!gliding)
                    rb.gravityScale = fallGravityScale;
            }

            if (Input.GetButtonDown("Vertical") && onPlatform && !buttonHold)
            {
                StartCoroutine(DownPlatform());
            }

            rb.velocity = currentVelocity;

            if ((currentVelocity.x > 0) && (transform.right.x < 0))
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else if ((currentVelocity.x < 0) && (transform.right.x > 0))
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Glide()
    {
        gliding = true;
        rb.gravityScale = 0.2f;
    }

    public IEnumerator EnterScene(float timeToEnter)
    {
        yield return new WaitForSeconds(timeToEnter);
        do
        {
            rb.velocity = new Vector3(2, 0, 0);

            yield return null;
        }
        while(transform.position.x < enterPoint.position.x);
        
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.0f);
        enteredScene = true;
    }

    public IEnumerator LeaveScene()
    {
        Lock();

        if (!onGround)
        {
            do
            {
                rb.gravityScale = fallGravityScale;

                yield return null;
            }
            while(rb.velocity != Vector2.zero);
        }
        
        do
        {
            rb.velocity = new Vector3(3, 0, 0);

            yield return null;
        }
        while(transform.position.x < leavePoint.position.x);
        
        rb.velocity = Vector3.zero;
    }

    public IEnumerator AdjustPosition(
        bool lookAtFollower = true, float distanceToAdjust = 0, float totalTime = 1.0f)
    {
        time = 0;

        if (!onGround)
        {
            do
            {
                rb.gravityScale = fallGravityScale;

                yield return null;
            }
            while(rb.velocity != Vector2.zero);
        }

        do
        {
            rb.velocity = new Vector3(distanceToAdjust, 0, 0);
            time += 1 * Time.deltaTime;
            yield return null;
        }
        while (time <= totalTime);

        if (lookAtFollower)
        {
            yield return new WaitForSeconds(0.5f);

            if (follower.position.x > gameObject.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                currentVelocity.x = -1;
            }
        }
    }

    public void Lock()
    {
        isLocked = true;
        jumping = false;
        gliding = false;
        buttonHold = false;
        sprite.sortingOrder = defaultLayer;

        rb.gravityScale = fallGravityScale;
        rb.velocity = new Vector3(0, currentVelocity.y, 0);
    }

    public void Unlock()
    {
        isLocked = false;
    }
    
    private bool IsOnGround()
    {
        Collider2D collider = Physics2D.OverlapBox(
            groundProbe.position, groundProbeSize, 0, groundMask);

        return (collider != null);
    }

    private bool IsOnPlatform()
    {
        Collider2D collider = Physics2D.OverlapBox(
            groundProbe.position, groundProbeSize, 0, platformMask);

        if (currentVelocity.y <= 0 && jumping 
            || currentVelocity.y <= 0 && gliding
            || !jumping && !gliding)
            return (collider != null && !requestingDown);

        return false;
    }

    private bool IsOnBound()
    {
        Collider2D collider = Physics2D.OverlapBox(
            groundProbe.position, groundProbeSize, 0, boundMask);

        return (collider != null);
    }

    private bool IsOnCoin()
    {
        Collider2D collider = Physics2D.OverlapBox(
            groundProbe.position, groundProbeSize, 0, coinMask);

        return (collider != null);
    }

    private bool IsHovering()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius * 9, platformMask);

        if (jumping || !onGround)
            return (collider != null && !platforming);

        return false;
    }
    
    private bool IsBalancingBack()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            platformProbeFront.position, platformProbeRadius, edgeMask);
        Collider2D colliderSec = Physics2D.OverlapCircle(
            platformProbeBack.position, platformProbeRadius, platformMask);

        return (collider != null && colliderSec == null && onPlatform);
    }

    private bool IsBalancingFront()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            platformProbeBack.position, platformProbeRadius, edgeMask);
        Collider2D colliderSec = Physics2D.OverlapCircle(
            platformProbeFront.position, platformProbeRadius, platformMask);

        return (collider != null && colliderSec == null && onPlatform);
    }

    private IEnumerator DownPlatform()
    {
        StartCoroutine(ResetDownRequest());

        platformCollider.enabled = false;

        do
        {
            yield return null;
        }
        while (!onPlatform && !onGround && !onBound && !onCoin);
        
        platformCollider.enabled = true;
    }

    private IEnumerator ResetDownRequest()
    {
        requestingDown = true;
        yield return new WaitForSeconds(0.2f);
        requestingDown = false;
    }

    private IEnumerator ResetGlideCooldown()
    {
        glideCooldown = glideCooldownAmount;

        do
        {
            glideCooldown -= 1 * Time.deltaTime;
            yield return null;
        }
        while (glideCooldown > 0 && !onGround && !onPlatform);

        glideCooldown = 0;
        hasPlayed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundProbe != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(groundProbe.position, groundProbeSize);
            Gizmos.DrawSphere(groundProbe.position, groundProbeRadius);
            Gizmos.DrawSphere(platformProbeFront.position, platformProbeRadius);
            Gizmos.DrawSphere(platformProbeBack.position, platformProbeRadius);
        }
    }

    public void Think()
    {
        if (!talking)
        {
            speechBalloon.ShowThoughtBalloon();
        }
        
        StartCoroutine(ResetTalk());
    }

    public void Talk()
    {
        if (!talking)
        {
            speechBalloon.ShowDialogueBalloon();
        }
        
        StartCoroutine(ResetTalk());
    }

    public void Listen() => speechBalloon.HideBalloon();

    private IEnumerator ResetTalk()
    {
        talking = true;
        yield return new WaitForEndOfFrame();
        talking = false;
    }

    public void Turn()
    {
        if (!turning)
        {
            if (transform.rotation == Quaternion.Euler(0, 0, 0))
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        StartCoroutine(ResetTurn());
    }

    private IEnumerator ResetTurn()
    {
        turning = true;
        yield return new WaitForEndOfFrame();
        turning = false;
    }
}
