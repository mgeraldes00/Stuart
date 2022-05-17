using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 offsetDefault;
    [SerializeField] private Vector2 speed = Vector2.one;

    [SerializeField] private float groundOffset;

    private void Start()
    {
        offsetDefault = offset;
        groundOffset = -gameObject.transform.localScale.x * 2;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 newPos;

            if (target.gameObject.GetComponent<Player>().CurrentVelocity.x > 0)
            {
                newPos = new Vector3(
                    target.position.x + offset.x, groundOffset, 0);
                offset = offsetDefault;
            }
            else if (target.gameObject.GetComponent<Player>().CurrentVelocity.x < 0)
            {
                newPos = new Vector3(
                    target.position.x - offset.x, groundOffset, 0);
                offset = -offsetDefault;
            }
            else
                newPos = newPos = new Vector3(
                    target.position.x + offset.x, groundOffset, 0);

            newPos.z = transform.position.z;

            Vector3 delta = new Vector3(
                newPos.x - transform.position.x, newPos.y, newPos.z);

            newPos.x = transform.position.x + delta.x * Time.deltaTime / speed.x;

            transform.position = newPos;
        }
    }
}
