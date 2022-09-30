using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = System.Random;

public class AgentMovement : MonoBehaviour
{
    private MobileAgent agent;

    [SerializeField] private float speed;

    private Rigidbody2D rb;

    private bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<MobileAgent>();
        rb = GetComponent<Rigidbody2D>();

        Rand rnd = new Rand();

        speed = rnd.Next(5, 8);

        if (agent.IsOnForeground)
            speed *= 3;

        if (agent.IsInverted)
            speed = -speed;

        StartCoroutine(Land());
    }

    // Update is called once per frame
    void Update()
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
}
