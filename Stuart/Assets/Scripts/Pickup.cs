using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private GameObject controllerObj;
    private IController controller;

    [SerializeField] private float rotation = 0;

    [SerializeField] float updateValue = 50;

    // Start is called before the first frame update
    private void Start()
    {
        controllerObj = GameObject.Find("Controller");
        controller = controllerObj.GetComponent<IController>();
    }

    // Update is called once per frame
    private void Update()
    {
        rotation += updateValue * Time.deltaTime;

        if (rotation > 180)
        {
            rotation = 180;
            updateValue = -updateValue;
        }

        if (rotation < 0)
        {
            rotation = 0;
            updateValue = -updateValue;
        }
        
        //gameObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            controller.CollectCoin();

            Destroy(gameObject);
        }
    }
}
