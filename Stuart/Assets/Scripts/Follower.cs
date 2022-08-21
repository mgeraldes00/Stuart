using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Player player;
    [SerializeField] private Transform enterPoint, leavePoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 offsetDefault;
    [SerializeField] private Vector2 speed = Vector2.one;
    [SerializeField] private Vector2 speedDefault = Vector2.one;
    [SerializeField] private Vector3 currentPos, currentVelocity;

    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private SpeechBalloon speechBalloon;

    [SerializeField] private Sprite[] sprites;

    [SerializeField] private bool isLocked;

    [SerializeField] private bool enteredScene;

    private bool talking, turning;

    private void Start()
    {
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        player = target.GetComponent<Player>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        speechBalloon = GetComponentInChildren<SpeechBalloon>();

        currentPos = transform.position;

        offsetDefault = offset;
        speedDefault = speed;
    }

    private void Update()
    {
        if (enteredScene && !isLocked)
        {
            UpdateMovement();

            currentVelocity = (transform.position - currentPos) / Time.deltaTime;
            currentPos = transform.position;
        }
        else
            currentVelocity = rb.velocity;
            

        /*if (transform.rotation.y == 0)
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
        else
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];*/

        animator.SetFloat("VelocityX", Mathf.Abs(currentVelocity.x));
    }

    private void UpdateMovement()
    {
        if (target != null)
        {
            Vector3 newPos = new Vector3(
                target.position.x + offset.x, gameObject.transform.position.y, 0);

            if (!player.Platforming && speed.x > speedDefault.x)
            {
                speed.x -= 1f * Time.deltaTime;
            }

            if (speed.x < speedDefault.x)
                speed.x = speedDefault.x;

            if (!player.Platforming && !player.HoveringPlatform)
            {
                if (player.CurrentVelocity.x > 0 || target.rotation.y == 0)
                {
                    offset = offsetDefault;
                    newPos = new Vector3(
                        target.position.x + offset.x, gameObject.transform.position.y, 0);
                    if (player.OnGround || player.Jumping || player.Gliding)
                    {
                        if (gameObject.transform.position.x < target.position.x
                            || speed.x == speedDefault.x)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                            animator.SetBool("movingRight", true);
                        }
                    }
                }
                else if (player.CurrentVelocity.x < 0 || target.rotation.y != 0)
                {
                    offset = -offsetDefault;
                    newPos = new Vector3(
                        target.position.x + offset.x, gameObject.transform.position.y, 0);
                    if (player.OnGround || player.Jumping || player.Gliding)
                    {
                        if (gameObject.transform.position.x > target.position.x
                            || speed.x == speedDefault.x)
                        {
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                            animator.SetBool("movingRight", false);
                        }
                    }
                }
            }
            else if (player.Platforming || player.HoveringPlatform)
            {
                if (currentVelocity.x > 1.5f || currentVelocity.x < -1.5f)
                    speed.x += speedDefault.x * 0.001f + Time.deltaTime;
                else
                    speed.x -= speedDefault.x * 0.001f + Time.deltaTime;
                if (speed.x > 1.5f)
                    speed.x = 1.5f;
                if (speed.x < 0.15f)
                    speed.x = 0.15f;

                if (player.CurrentVelocity.x > 0
                    && gameObject.transform.position.x 
                    < target.gameObject.transform.position.x
                    || player.CurrentVelocity.x > 0 && currentVelocity.x > 0
                    || player.HoveringPlatform 
                    && gameObject.transform.position.x 
                    < target.gameObject.transform.position.x)
                {
                    offset = offsetDefault * 5;
                    newPos = new Vector3(
                        target.position.x + offset.x, 
                        gameObject.transform.position.y, 0);
                    
                }
                else if (player.CurrentVelocity.x < 0
                    && gameObject.transform.position.x > 
                    target.gameObject.transform.position.x
                    || player.CurrentVelocity.x < 0 && currentVelocity.x < 0
                    || player.HoveringPlatform
                    && gameObject.transform.position.x 
                    > target.gameObject.transform.position.x)
                {
                    offset = -offsetDefault * 5;
                    newPos = new Vector3(
                        target.position.x + offset.x, 
                        gameObject.transform.position.y, 0);
                }
                if (gameObject.transform.position.x <
                    target.gameObject.transform.position.x + (offset.x + 0.1f)
                    && offset.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    animator.SetBool("movingRight", true);
                }
                else if (gameObject.transform.position.x >
                    target.gameObject.transform.position.x + (offset.x - 0.1f)
                    && offset.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    animator.SetBool("movingRight", false);
                } 
            }
            else
            {   	
                newPos = new Vector3(
                    target.position.x + offset.x, gameObject.transform.position.y, 0);
            }                

            newPos.z = transform.position.z;

            Vector3 delta = new Vector3(
                newPos.x - transform.position.x, newPos.y, newPos.z);
                    
            newPos.x = 
                transform.position.x + delta.x * Time.deltaTime / speed.x;

            transform.position = newPos;
        }
    }

    public IEnumerator EnterScene(float timeToEnter)
    {
        yield return new WaitForSeconds(timeToEnter);

        if (enterPoint.position.x < transform.position.x)
        {
            do
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                animator.SetBool("movingRight", false);
                rb.velocity = new Vector3(-2, 0, 0);
                yield return null;
            }
            while (transform.position.x > enterPoint.position.x);
        }
        else
        {
            do
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetBool("movingRight", true);
                rb.velocity = new Vector3(2, 0, 0);
                yield return null;
            }
            while (transform.position.x < enterPoint.position.x - 0.3f);
        }
        
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.0f);
        enteredScene = true;
    }

    public IEnumerator LeaveScene()
    {
        Lock();

        do
        {
            rb.velocity = new Vector3(3, 0, 0);

            yield return null;
        }
        while(transform.position.x < leavePoint.position.x);
        
        rb.velocity = Vector3.zero;
    }

    public IEnumerator AdjustPosition(float distanceToAdjust = 5)
    {
        float targetPosition = 0;
        float targetSpeed;

        if (target.position.x > gameObject.transform.position.x)
        {
            targetPosition = target.position.x - distanceToAdjust;
            if (gameObject.transform.position.x - targetPosition > distanceToAdjust)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetBool("movingRight", true);
                targetSpeed = 3;

                do
                {
                    rb.velocity = new Vector3(targetSpeed, 0, 0);
                    yield return null;
                }
                while (transform.position.x < targetPosition);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                animator.SetBool("movingRight", false);
                targetSpeed = -3;

                do
                {
                    rb.velocity = new Vector3(targetSpeed, 0, 0);
                    yield return null;
                }
                while (transform.position.x > targetPosition);
            }

            transform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetBool("movingRight", true);
        }
        else
        {
            targetPosition = target.position.x + distanceToAdjust;
            if (targetPosition - gameObject.transform.position.x > distanceToAdjust)
            {
                targetSpeed = -3;

                do
                {
                    rb.velocity = new Vector3(targetSpeed, 0, 0);
                    yield return null;
                }
                while (transform.position.x > targetPosition);
            }
            else
            {
                targetSpeed = 3;

                do
                {
                    rb.velocity = new Vector3(targetSpeed, 0, 0);
                    yield return null;
                }
                while (transform.position.x < targetPosition);
            }

            transform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("movingRight", false);
        }

        rb.velocity = Vector3.zero;
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        speed.x = 1.25f;

        isLocked = false;
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
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                animator.SetBool("movingRight", false);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetBool("movingRight", true);
            } 
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
