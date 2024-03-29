using System.Collections;
using UnityEngine;
using Rand = UnityEngine.Random;

public class AgentMovement : MonoBehaviour
{
    private const float maxSpeed = 8f;

    private MobileAgent agent;

    [SerializeField] private float speed;

    private Rigidbody2D rb;

    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer objectRenderer;
    private Sprite currentSprite;

    [SerializeField] private AudioSource[] steps;

    private bool onGround;

    private void Start()
    {
        agent = GetComponent<MobileAgent>();
        rb = GetComponent<Rigidbody2D>();
        objectRenderer = GetComponent<SpriteRenderer>();

        speed = Rand.Range(5f, maxSpeed);

        if (agent.IsOnForeground)
            speed *= 3;

        if (agent.IsInverted)
            speed = -speed;

        StartCoroutine(Land());

        StartCoroutine(MoveAnimation());
    }

    private void Update()
    {
        if (onGround)
            rb.velocity = new Vector3(speed, 0, 0);
    }

    private IEnumerator Land()
    {
        yield return new WaitForSeconds(0.1f);
        while(rb.velocity != Vector2.zero)
        {
            rb.gravityScale = rb.gravityScale * 5;
            yield return null;
        }

        rb.gravityScale = 1;
        onGround = true;
    }

    private IEnumerator MoveAnimation()
    {
        int nextInLine = 0, nextStep = 0;
        float moveSpeed = speed;

        if (speed < 0)
            moveSpeed = -speed;

        if (speed > 10)
            moveSpeed = speed / 3;

        float moveSpeedPercent = moveSpeed / maxSpeed;
        
        float moveSpeedFinal = 1 - moveSpeedPercent;

        if (moveSpeedFinal < 0.2f)
            moveSpeedFinal = 0.2f;

        while (true)
        {
            currentSprite = sprites[nextInLine];
            if (nextInLine == 1) steps[nextInLine].Play();
            objectRenderer.sprite = currentSprite;

            yield return new WaitForSeconds(moveSpeedFinal);

            nextInLine++;
            nextStep++;

            if (nextInLine >= sprites.Length) nextInLine = 0;
            if (nextStep >= steps.Length) nextStep = 0;
        }
    }
}
