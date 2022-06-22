using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform enterPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 offsetDefault;
    [SerializeField] private Vector2 speed = Vector2.one;
    [SerializeField] private Vector2 speedDefault = Vector2.one;

    private Rigidbody2D rb;
    [SerializeField] private SpeechBalloon speechBalloon;

    [SerializeField] private bool isLocked;

    [SerializeField] private bool enteredScene;

    private void Start()
    {
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();

        rb = GetComponent<Rigidbody2D>();
        speechBalloon = GetComponentInChildren<SpeechBalloon>();

        offsetDefault = offset;
        speedDefault = speed;
    }

    private void Update()
    {
        if (enteredScene && !isLocked)
            UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (target != null)
        {
            Vector3 newPos = new Vector3(
                target.position.x + offset.x, gameObject.transform.position.y, 0);

            if (target.gameObject.GetComponent<Player>().OnGround
                && speed.x > speedDefault.x)
            {
                speed.x -= 1f * Time.deltaTime;
            }

            if (speed.x < speedDefault.x)
                speed.x = speedDefault.x;

            if (!target.gameObject.GetComponent<Player>().OnPlatform)
            {
                if (target.gameObject.GetComponent<Player>().CurrentVelocity.x > 0)
                {
                    newPos = new Vector3(
                        target.position.x + offset.x, gameObject.transform.position.y, 0);
                    offset = offsetDefault;
                    if (target.gameObject.GetComponent<Player>().OnGround
                        || target.gameObject.GetComponent<Player>().Jumping
                        || target.gameObject.GetComponent<Player>().Gliding)
                    {
                        if (gameObject.transform.position.x 
                            < target.gameObject.transform.position.x
                            || speed.x == speedDefault.x)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                    }
                }
                else if (target.gameObject.GetComponent<Player>().CurrentVelocity.x < 0)
                {
                    newPos = new Vector3(
                        target.position.x + offset.x, gameObject.transform.position.y, 0);
                    offset = -offsetDefault;
                    if (target.gameObject.GetComponent<Player>().OnGround
                        || target.gameObject.GetComponent<Player>().Jumping
                        || target.gameObject.GetComponent<Player>().Gliding)
                    {
                        if (gameObject.transform.position.x 
                            > target.gameObject.transform.position.x
                            || speed.x == speedDefault.x)
                        {
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                    }
                }
            }
            else if (target.gameObject.GetComponent<Player>().OnPlatform)
            {
                speed.x += speedDefault.x * 0.001f + Time.deltaTime;
                if (speed.x > 1.5f)
                    speed.x = 1.5f;
                if (target.gameObject.GetComponent<Player>().CurrentVelocity.x > 0
                    && gameObject.transform.position.x < target.gameObject.transform.position.x)
                {
                    newPos = new Vector3(
                        target.position.x + offset.x, gameObject.transform.position.y, 0);
                    offset = offsetDefault;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (target.gameObject.GetComponent<Player>().CurrentVelocity.x < 0
                    && gameObject.transform.position.x > target.gameObject.transform.position.x)
                {
                    newPos = new Vector3(
                        target.position.x + offset.x, gameObject.transform.position.y, 0);
                    offset = -offsetDefault;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
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

            newPos.x = transform.position.x + delta.x * Time.deltaTime / speed.x;

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
                rb.velocity = new Vector3(2, 0, 0);
                yield return null;
            }
            while (transform.position.x < enterPoint.position.x);
        }
        
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.0f);
        enteredScene = true;
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
        speechBalloon.ShowBalloon();
    }

    public void Listen()
    {
        speechBalloon.HideBalloon();
    }
}
