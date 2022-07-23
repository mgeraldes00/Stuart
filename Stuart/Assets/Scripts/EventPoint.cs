using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    [SerializeField] private GameObject controllerObj;
    private IController controller;

    [SerializeField] private int eventIndex;

    private bool interacted;

    // Start is called before the first frame update
    private void Start()
    {
        controllerObj = GameObject.Find("Controller");
        controller = controllerObj.GetComponent<IController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!interacted)
            {
                Debug.Log("Collided with player");
                controller.BeginEvent(eventIndex);
            }

            interacted = true;
            Destroy(gameObject);
        }
    }
}
