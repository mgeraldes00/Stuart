using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private float[] xyPosition = new float[2];

    [SerializeField] private bool horizontal;

    [SerializeField] private float maxMovement;

    [SerializeField] private float updateValue;

    [SerializeField] private float movement;

    // Start is called before the first frame update
    void Awake()
    {
        xyPosition[0] = transform.position.x;
        xyPosition[1] = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        xyPosition[0] = transform.position.x;
        xyPosition[1] = transform.position.y;

        movement += updateValue * Time.deltaTime;
        
        if (movement > maxMovement)
        {
            movement = maxMovement;
            updateValue = -updateValue;
        }

        if (movement < -maxMovement)
        {
            movement = -maxMovement;
            updateValue = -updateValue;
        }

        if (horizontal)
            transform.position = new Vector3(
                xyPosition[0] + updateValue, xyPosition[1], 0);
        else
            transform.position = new Vector3(
                xyPosition[0], xyPosition[1] + updateValue, 0);
    }
}
