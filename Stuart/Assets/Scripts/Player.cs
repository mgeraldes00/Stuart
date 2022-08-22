using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private const float glideCooldownAmount = 0.5f;
    /*private readonly float[] colliderSize = new float[]{1f, 0.2f};
    private readonly float[] colliderSizeJump = new float[]{2f, 0.2f};
    private readonly float[] colliderSizeJumpAlt = new float[]{1.75f, 2f};
    private readonly float[] colliderSizePlatform = new float[]{1f, 2f};
    private float colliderInc;*/

    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField] private Transform follower;
    [SerializeField] private Transform groundProbe;
    [SerializeField] private Transform enterPoint, leavePoint;
    [SerializeField] private float groundProbeRadius = 5.0f;
    [SerializeField] private Vector2 groundProbeSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask platformMask;
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
    //private CapsuleCollider2D bodyCollider;
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
        //bodyCollider = GetComponent<CapsuleCollider2D>();

        speechBalloon = GetComponentInChildren<SpeechBalloon>();

        follower = GameObject.FindWithTag("Follower").GetComponent<Transform>();

        onGround = true;
    }

    private void Update()
    {
        currentVelocity = rb.velocity;
        onGround = IsOnGround();
        onPlatform = IsOnPlatform();
        hoveringPlatform = IsHovering();

        if (onPlatform)
        {
            sprite.sortingOrder = defaultLayer - 2;
            platforming = true;
            /*if (colliderDecrease)
                StartCoroutine(ColliderIncrease(false, 1.5f));*/
            //colliderInc = colliderSize[0];
        }
            
        if (onGround)
        {
            sprite.sortingOrder = defaultLayer;
            platforming = false;
            /*if (colliderDecrease)
                StartCoroutine(ColliderIncrease(false));*/
            //colliderInc = colliderSize[0];
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
    }

    private void UpdateMovement()
    {
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

            if (onPlatform || onGround)
            {
                jumping = false;
                gliding = false;
                hasPlayed = false;
                glideCooldown = 0;
                /*bodyCollider.size = 
                    new Vector2(colliderSize[0], colliderSize[1]);*/
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
                        //StartCoroutine(ResetGlideCooldown());
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
                        //StartCoroutine(ResetGlideCooldown());
                    }
                    
                    hasPlayed = true;
                }
            }
            else if (Input.GetButtonUp("Jump"))
            {
                jumping = false;
                //hasPlayed = false;
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
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if ((currentVelocity.x < 0) && (transform.right.x > 0))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private void Glide()
    {
        gliding = true;
        rb.gravityScale = 0.2f;
        //if (currentVelocity.y < -5 * fallGravityScale)
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
        //rb.velocity = Vector3.zero;
    }

    public void Unlock()
    {
        isLocked = false;
    }
    
    private bool IsOnGround()
    {
        /*Collider2D collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius, groundMask);*/

        Collider2D collider = Physics2D.OverlapBox(
            groundProbe.position, groundProbeSize, 0, groundMask);

        return (collider != null);
    }

    private bool IsOnPlatform()
    {
        /*Collider2D collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius, platformMask);*/

        Collider2D collider = Physics2D.OverlapBox(
            groundProbe.position, groundProbeSize, 0, platformMask);

        /*Collider2D collider = Physics2D.OverlapCapsule(
            groundProbe.position, groundProbeSize, 
            CapsuleDirection2D.Horizontal, 0, platformMask);*/

        if (currentVelocity.y <= 0 && jumping 
            || currentVelocity.y <= 0 && gliding
            || !jumping && !gliding)
            return (collider != null && !requestingDown);

        return false;
    }

    private bool IsHovering()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius * 9, platformMask);

        if (jumping || !onGround)
            return (collider != null && !platforming);

        return false;
    }

    private IEnumerator DownPlatform()
    {
        StartCoroutine(ResetDownRequest());

        //bodyCollider.enabled = false;
        platformCollider.enabled = false;

        do
        {
            yield return null;
        }
        while (!onPlatform && !onGround);
        
        //bodyCollider.enabled = true;
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
            Gizmos.DrawSphere(groundProbe.position, groundProbeRadius);
            Gizmos.DrawCube(groundProbe.position, groundProbeSize);
            /*Gizmos.DrawMesh(
                probeMesh, 0, groundProbe.position, Quaternion.Euler(0, 0, 0), 
                groundProbeSize);*/
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

    public void Listen()
    {
        speechBalloon.HideBalloon();
    }

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
