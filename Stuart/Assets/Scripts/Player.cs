using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = 20.0f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentVelocity = rb.velocity;

        float hAxis = Input.GetAxis("Horizontal");
        currentVelocity.x = hAxis * horizontalSpeed;

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
