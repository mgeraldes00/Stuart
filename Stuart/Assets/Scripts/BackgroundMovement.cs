using System.Collections;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private float[] xyPosition = new float[2];
    [SerializeField] private float[] scalePos = new float[3];

    [SerializeField] private bool horizontal, isScale, paused, canPause;

    [SerializeField] private float maxMovement, updateValue, waitTime;
    [SerializeField] private float movement;

    // Start is called before the first frame update
    void Awake()
    {
        if (!isScale)
        {
            xyPosition[0] = transform.position.x;
            xyPosition[1] = transform.position.y;
        }
        else
        {
            scalePos[0] = transform.localScale.x;
            scalePos[1] = transform.localScale.y;
            scalePos[2] = transform.localScale.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isScale)
        {
            xyPosition[0] = transform.position.x;
            xyPosition[1] = transform.position.y;
        }
        else
        {
            scalePos[0] = transform.localScale.x;
            scalePos[1] = transform.localScale.y;
            scalePos[2] = transform.localScale.z;
        }
        
        if (!paused)
        {
            movement += updateValue * Time.deltaTime;

            if (movement > maxMovement)
            {
                movement = maxMovement;
                updateValue = -updateValue;

                if (canPause) StartCoroutine(Pause(waitTime));
            }

            if (movement < -maxMovement)
            {
                movement = -maxMovement;
                updateValue = -updateValue;

                if (canPause && horizontal) 
                    StartCoroutine(Pause(waitTime * 1.5f));
            }

            if (!isScale)
            {
                if (horizontal)
                    transform.position = new Vector3(
                        xyPosition[0] + updateValue, xyPosition[1], 0);
                else
                    transform.position = new Vector3(
                        xyPosition[0], xyPosition[1] + updateValue, 0);
            }
            else
            {
                transform.localScale = new Vector3(
                    scalePos[0] + updateValue, 
                    scalePos[1] + updateValue, 
                    scalePos[2] + updateValue);
            }
        }
    }

    private IEnumerator Pause(float time)
    {
        paused = true;
        canPause = false;
        yield return new WaitForSeconds(time);
        paused = false;
        yield return new WaitForEndOfFrame();
        canPause = true;
    }
}
