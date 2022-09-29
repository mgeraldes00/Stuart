using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = System.Random;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        Rand rnd = new Rand();

        speed = rnd.Next(2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
