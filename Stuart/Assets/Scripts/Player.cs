using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField] private Transform follower;
    [SerializeField] private Transform groundProbe;
    [SerializeField] private Transform enterPoint, leavePoint;
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
    private Animator animator;

    [SerializeField] private SpeechBalloon speechBalloon;

    [SerializeField] private AudioSource[] sounds;

    private float jumpTime;
    [SerializeField] private float glideCooldown;
    [SerializeField] private float inputLockTimer = 0;
    [SerializeField] private bool onGround;
    public bool OnGround => onGround;
    [SerializeField] private bool onPlatform;
    public bool OnPlatform => onPlatform;
    [SerializeField] private bool jumping;
    public bool Jumping => jumping;
    [SerializeField] private bool gliding;
    public bool Gliding => gliding;
    [SerializeField] private bool requestingDown;

    [SerializeField] private bool isLocked;
    public bool IsLocked => isLocked;

    [SerializeField] private bool enteredScene;

    private bool hasPlayed;

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

        if (enteredScene && !isLocked)
        {
            UpdateMovement();
        }

        animator.SetFloat("VelocityX", Mathf.Abs(currentVelocity.x));
        animator.SetFloat("VelocityY", currentVelocity.y);
        animator.SetFloat("Gravity", rb.gravityScale);
        animator.SetBool("grounded", onGround);
        animator.SetBool("onPlatform", onPlatform);
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

            if (onPlatform)
            {
                jumping = false;
                gliding = false;
                hasPlayed = false;
                sprite.sortingOrder = defaultLayer - 2;
            }
            else if (onGround)
            {
                jumping = false;
                gliding = false;
                hasPlayed = false;
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
                    if (currentVelocity.y <= 0 && glideCooldown <= 0)
                    {
                        gliding = true;
                        rb.gravityScale = 0.2f;
                        //if (currentVelocity.y < -5 * fallGravityScale)
                        currentVelocity.y -= currentVelocity.y * 0.8f;

                        sounds[0].Play();
                        StartCoroutine(ResetGlideCooldown());
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
                    jumping = true;
                if (jumping && currentVelocity.y <= 0)
                {
                    gliding = true;
                    rb.gravityScale = 0.2f;

                    if (!hasPlayed)
                    {
                        sounds[0].Play();
                        //StartCoroutine(ResetGlideCooldown());
                    }
                    
                    hasPlayed = true;
                }
            }
            else if (Input.GetButtonUp("Jump"))
            {
                jumping = false;

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

            if (Input.GetButtonDown("Vertical") && onPlatform)
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

    public IEnumerator AdjustPosition(float distanceToAdjust = 0)
    {
        float time = 0;

        do
        {
            rb.velocity = new Vector3(distanceToAdjust, 0, 0);
            time += 0.01f;
            yield return null;
        }
        while (time <= 1);

        yield return new WaitForSeconds(0.5f);
        if (follower.position.x > gameObject.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            currentVelocity.x = -1;
        }
    }

    public void Lock()
    {
        isLocked = true;

        rb.gravityScale = fallGravityScale;
        //rb.velocity = Vector3.zero;
    }

    public void Unlock()
    {
        isLocked = false;
    }
    
    private bool IsOnGround()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius, groundMask);

        return (collider != null);
    }

    private bool IsOnPlatform()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            groundProbe.position, groundProbeRadius, platformMask);

        return (collider != null && !requestingDown);
    }

    private IEnumerator DownPlatform()
    {
        StartCoroutine(ResetDownRequest());

        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

        do
        {
            yield return null;
        }
        while (!onPlatform && !onGround);
        
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
    }

    private IEnumerator ResetDownRequest()
    {
        requestingDown = true;
        yield return new WaitForSeconds(0.2f);
        requestingDown = false;
    }

    private IEnumerator ResetGlideCooldown()
    {
        glideCooldown = 0.1f;

        do
        {
            glideCooldown -= 1 * Time.deltaTime;
            yield return null;
        }
        while (glideCooldown > 0 && !onGround && !onPlatform);

        glideCooldown = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundProbe != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundProbe.position, groundProbeRadius);
        }
    }

    public void Talk()
    {
        speechBalloon.ShowBalloon();
    }

    public void Listen()
    {
        speechBalloon.HideBalloon();
    }

    public void Turn()
    {
        if (transform.rotation == Quaternion.Euler(0, 0, 0))
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
