using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    [SerializeField] private GameObject controllerObj;
    private IController controller;

    [SerializeField] private int eventIndex;

    // Start is called before the first frame update
    private void Start()
    {
        controllerObj = GameObject.Find("Controller");
        controller = controllerObj.GetComponent<IController>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            controller.BeginEvent(eventIndex);
        }
    }
}
